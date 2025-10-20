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
    public class TicketConfigurations : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.ToTable("Tickets");

            builder.HasKey(t => t.ID);

            builder.Property(t => t.Place)
                .HasMaxLength(70)
                .IsRequired(true)
                .HasColumnType("varchar");

            
            
            builder.Property(t => t.Type)
                .HasMaxLength(50)
                .IsRequired(true)
                .HasColumnType("varchar");


            builder.HasOne(t => t.Booking)
                .WithMany(b => b.Tickets)
                .HasForeignKey(t => t.BookingId)
                .OnDelete(DeleteBehavior.SetNull);

            //Foreign Key => EventId
            //Foreign Key => CategoryId

        }
    }

}
