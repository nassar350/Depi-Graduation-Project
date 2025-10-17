using Eventify.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventify.Core.Configurations;

public class UsersConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> user)
    {
        user.ToTable("Users");
        user.HasKey(U => U.Id);
        
        // User ==> Events  (One To Many)
        // Remember Deletion 

        user.HasMany(u => u.Events)
            .WithOne(e => e.Organizer)
            .HasForeignKey(e => e.OrganizerID);
    }
}