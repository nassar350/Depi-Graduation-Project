using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventify.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoUrlToEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photo",
                table: "Events");

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "Events",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "Events");

            migrationBuilder.AddColumn<byte[]>(
                name: "Photo",
                table: "Events",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
