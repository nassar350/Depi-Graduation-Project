using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventify.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingCategoryNametoBookingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "Bookings",
                type: "varchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "Bookings");
        }
    }
}
