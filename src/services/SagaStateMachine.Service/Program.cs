using Microsoft.EntityFrameworkCore;
using SagaStateMachine.Service;

var builder = Host.CreateApplicationBuilder(args);

SagaConfiguration.ConfigureMassTransit(builder.Services, builder.Configuration);

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<DbContext>().Database.EnsureCreated();
}

host.Run();
