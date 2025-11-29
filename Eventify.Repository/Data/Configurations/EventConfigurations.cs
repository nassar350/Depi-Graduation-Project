using Eventify.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventify.Core.Configurations;

public class EventConfigurations : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("Events");
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

        builder.Property(p => p.EventCategory)
                .HasMaxLength(50)
                .IsRequired(true)
                .HasConversion<string>();

        builder.Property(ev => ev.PhotoUrl)
       .HasColumnName("PhotoUrl")
       .HasMaxLength(1000)
       .IsRequired(false);

        // Event => Categories (One TO Many)
        builder.HasMany(u => u.Categories)
            .WithOne(cat => cat.Event)
            .HasForeignKey(cat => cat.EventId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(ev => ev.Tickets)
            .WithOne(ticket =>  ticket.Event)
            .HasForeignKey(ticket => ticket.EventId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(e => e.Name);
        builder.HasIndex(e => e.StartDate);
        builder.HasIndex(e => e.EndDate);
        builder.HasIndex(e => new { e.Name, e.StartDate }); 
    }
}