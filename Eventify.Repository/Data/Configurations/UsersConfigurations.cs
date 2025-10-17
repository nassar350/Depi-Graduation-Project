using Eventify.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventify.Core.Configurations;

public class UsersConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(U => U.Id);
        
        builder.Property(user => user.Email)
            .HasMaxLength(100)
            .IsRequired(true)
            .HasColumnType("varchar");

        
        
        builder.Property(user => user.Name)
            .HasMaxLength(70)
            .IsRequired(true)
            .HasColumnType("varchar");


        builder.Property(user => user.Phone)
            .IsRequired(true)
            .HasMaxLength(20)
            .HasColumnType("varchar");

        builder.Property(user => user.Password)
            .HasMaxLength(100)
            .HasColumnType("varchar");

        builder.Property(user => user.Role)
            .IsRequired()
            .HasConversion<string>();


        // User ==> Events  (One To Many)
        // Remember Deletion

        builder.HasMany(u => u.Events)
            .WithOne(e => e.Organizer)
            .HasForeignKey(e => e.OrganizerID)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasMany(user  => user.Bookings)
            .WithOne(book => book.User)
            .HasForeignKey(book => book.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        
        
    }
}