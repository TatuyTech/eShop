using eShop.Data.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eShop.Data.Config;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.OwnsOne(i => i.ItemOrdered, io =>
        {
            io.WithOwner();

            io.Property(cio => cio.ProductName)
                .HasMaxLength(50)
                .IsRequired();
        });

        builder.Property(oi => oi.UnitPrice)
            .IsRequired(true)
            .HasColumnType("decimal(18,2)");
    }
}
