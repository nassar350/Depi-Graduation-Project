using Eventify.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventify.Core.Configurations;

public class CategoryConfigurations : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
        
    {
        builder.ToTable("Categories");
        
        builder.HasKey(category => category.Id);
        
        builder.Property(category => category.Title)
            .HasMaxLength(50)
            .IsRequired(true)
            .HasColumnType("varchar");


        builder.Property(category => category.Seats)
            .IsRequired(true); 
        
        builder.Property(category => category.Booked)
            .IsRequired(true);
        
        builder.HasMany(Category => Category.Tickets)
            .WithOne(ticket =>  ticket.Category)
            .HasForeignKey(ticket => ticket.CategoryId )
            .OnDelete(DeleteBehavior.Cascade);
        
        




    }
}