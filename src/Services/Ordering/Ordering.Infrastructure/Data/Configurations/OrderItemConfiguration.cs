using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Models;
using Ordering.Domain.ValueObjects;

namespace Ordering.Infrastructure.Data.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(oi => oi.Id);
        builder.Property(oi => oi.Id).HasConversion(
            orderItemId => orderItemId.Value,
            dbId=> OrderItemId.Of(dbId));
        builder.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId);
        builder.Property(x=>x.Quantity).IsRequired();
        builder.Property(x=>x.Price).IsRequired();
    }
}
