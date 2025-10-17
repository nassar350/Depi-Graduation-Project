using Eventify.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventify.Core.Configurations;

public class UserAttendEventConfigurations : IEntityTypeConfiguration<UserAttendEvent>
{
    public void Configure(EntityTypeBuilder<UserAttendEvent> builder)
    {
        builder.HasKey(O => new    // CPK
        {
            O.Event_Id, O.UserId
        });

        builder.HasOne(US => US.User)
            .WithMany(E => E.UserAttendEvents)
            .HasForeignKey(U => U.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(E => E.Event)
            .WithMany(E => E.EventsAttendedByUsers)
            .HasForeignKey(E => E.Event_Id)
            .OnDelete(DeleteBehavior.NoAction);

        builder.ToTable("UserAttendEvents");



    }
}