using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SagaStateMachine.Service;

public class OrderStateMap : SagaClassMap<OrderStateInstance>
{
    protected override void Configure(EntityTypeBuilder<OrderStateInstance> entity, ModelBuilder model)
    {
        entity.Property(x => x.BuyerId).IsRequired();
        entity.Property(x => x.OrderId).IsRequired();
        entity.Property(x => x.TotalPrice).HasDefaultValue(0);
    }
}