using Eventify.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Repository.Configurations
{
    public class PaymentConfigurations: IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(p => p.BookingId);

            builder.Property(p => p.TotalPrice).HasColumnType("decimal(5,2").IsRequired();
            builder.Property(p => p.PaymentMethod).HasMaxLength(50).IsRequired();
            builder.Property(p => p.Status).HasMaxLength(50).IsRequired();
            builder.Property(p => p.DateTime).IsRequired();

            builder.HasOne(p => p.Booking)
                .WithOne(b =>b.Payment).
                HasForeignKey<Payment>(p => p.BookingId)
                .IsRequired();


        }
    }
}
