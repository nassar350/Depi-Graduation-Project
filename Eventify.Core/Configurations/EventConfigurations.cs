using Eventify.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventify.Core.Configurations;

public class EventConfigurations : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> E)
    {
        E.ToTable("Events");
        E.HasKey(e => e.Id);
        
        // Event => Categories (One TO Many)
        E.HasMany(u => u.Categories)
            .WithOne(cat => cat.Event)
            .HasForeignKey(cat => cat.Id);
       
    }
}