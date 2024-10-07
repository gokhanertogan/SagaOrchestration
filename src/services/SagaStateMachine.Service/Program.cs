using SagaStateMachine.Service;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

SagaConfiguration.ConfigureMassTransit(builder.Services, builder.Configuration);

var host = builder.Build();

host.Run();
