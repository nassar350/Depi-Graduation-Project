using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventify.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingTicketPriceToCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TicketPrice",
                table: "Categories",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Events_EndDate",
                table: "Events",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Events_Name",
                table: "Events",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Events_Name_StartDate",
                table: "Events",
                columns: new[] { "Name", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Events_StartDate",
                table: "Events",
                column: "StartDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Events_EndDate",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_Name",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_Name_StartDate",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_StartDate",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "TicketPrice",
                table: "Categories");
        }
    }
}
