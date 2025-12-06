using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventify.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingEventIdinBookingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Bookings");
        }
    }
}
