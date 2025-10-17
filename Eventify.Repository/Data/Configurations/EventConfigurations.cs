using Eventify.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventify.Core.Configurations;

public class EventConfigurations : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("buildervents");
        builder.HasKey(e => e.Id);

        builder.Property(ev => ev.Name)
            .HasMaxLength(50)
            .HasColumnType("varchar")
            .IsRequired(true);
        
        
        builder.Property(ev => ev.Description)
            .HasMaxLength(999)
            .HasColumnType("varchar")
            .IsRequired(true); 
        
        builder.Property(ev => ev.Address)
            .HasMaxLength(70)
            .HasColumnType("varchar")
            .IsRequired(true); 
        
        builder.Property(ev => ev.StartDate)
            .IsRequired() ;
        
        builder.Property(ev => ev.EndDate)
            .IsRequired() ;

        builder.Property(ev => ev.Photo)
            .IsRequired(false);
        

        // Event => Categories (One TO Many)
        builder.HasMany(u => u.Categories)
            .WithOne(cat => cat.Event)
            .HasForeignKey(cat => cat.Id)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(ev => ev.Tickets)
            .WithOne(ticket =>  ticket.Event)
            .HasForeignKey(ticket => ticket.EventId )
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(ev => ev.Bookings)
            .WithOne(booking =>  booking.Event)
            .HasForeignKey(ticket => ticket.EventId )
            .OnDelete(DeleteBehavior.Cascade);

    }
}