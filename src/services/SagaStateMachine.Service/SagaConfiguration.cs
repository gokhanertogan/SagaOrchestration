using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Settings;

namespace SagaStateMachine.Service;

public static class SagaConfiguration
{
    public static void ConfigureMassTransit(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(configure =>
        {
            configure.AddSagaStateMachine<OrderStateMachine, OrderStateInstance>()
                .EntityFrameworkRepository(options =>
                {
                    options.AddDbContext<DbContext, OrderStateDbContext>((provider, builder) =>
                    {
                        builder.UseSqlServer(configuration.GetConnectionString("ConnectionString"));
                    });
                });

            configure.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration.GetConnectionString("RabbitMQ"));
                cfg.ReceiveEndpoint(RabbitMQSettings.StateMachine,
                                   e => e.ConfigureSaga<OrderStateInstance>(context));
            });
        });
    }
}