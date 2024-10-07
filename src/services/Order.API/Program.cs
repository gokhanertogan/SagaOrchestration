using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Consumers;
using Order.API.Contexts;
using Order.API.Models;
using Order.API.Services;
using Shared.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(configure =>
{
    configure.AddConsumer<OrderCompletedEventConsumer>();
    configure.AddConsumer<OrderFailedEventConsumer>();

    configure.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(builder.Configuration["RabbitMQ"]);

        configurator.ReceiveEndpoint(RabbitMQSettings.Order_OrderCompletedEventQueue,
                                     e => e.ConfigureConsumer<OrderCompletedEventConsumer>(context));
        configurator.ReceiveEndpoint(RabbitMQSettings.Order_OrderFailedEventQueue,
                                     e => e.ConfigureConsumer<OrderFailedEventConsumer>(context));

    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/create-order", async (IOrderService orderService, OrderCreateRequestModel requestModel) =>
{
    if (requestModel is null)
    {
        return Results.BadRequest("Request model cannot be null.");
    }

    await orderService.CreateOrderAsync(requestModel);
    return Results.Ok();
});

app.Run();