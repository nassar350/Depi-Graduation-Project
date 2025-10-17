using Eventify.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Repository.Data.Configurations
{
    public class PaymentConfigurations : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(p => p.BookingId);

            builder.Property(p => p.TotalPrice)
                .HasColumnType("decimal(10,2")
                .IsRequired(true); 
            
            
            
            builder.Property(p => p.PaymentMethod)
                .HasMaxLength(50)
                .IsRequired(true);


            builder.Property(p => p.Status)
                .HasMaxLength(50)
                .IsRequired(true)
                .HasConversion<string>();
            
            
            builder.Property(p => p.DateTime)
                .IsRequired(true);

           
            
            builder.HasOne(p => p.Booking)
                .WithOne(b => b.Payment)
                .HasForeignKey<Payment>(p => p.BookingId)
                .OnDelete(DeleteBehavior.NoAction);



        }
    }

}
