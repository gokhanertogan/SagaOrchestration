using MassTransit;
using Shared.Settings;
using Stock.API;
using Stock.API.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(configure =>
{
    configure.AddConsumer<OrderCreatedEventConsumer>();
    configure.AddConsumer<StockRollbackMessageConsumer>();

    configure.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(builder.Configuration["RabbitMQ"]);

        configurator.ReceiveEndpoint(RabbitMQSettings.Stock_OrderCreatedEventQueue,
                                     e => e.ConfigureConsumer<OrderCreatedEventConsumer>(context));
        configurator.ReceiveEndpoint(RabbitMQSettings.Stock_RollbackMessageQueue,
                                     e => e.ConfigureConsumer<StockRollbackMessageConsumer>(context));

    });
});

builder.Services.AddSingleton<MongoDbService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var mongoDbService = services.GetRequiredService<MongoDbService>();
    var dataInitializer = new DataInitializer(mongoDbService);
    dataInitializer.SeedData();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
