using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventify.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddZoomFieldsToEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOnline",
                table: "Events",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ZoomJoinUrl",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZoomMeetingId",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZoomPassword",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOnline",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ZoomJoinUrl",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ZoomMeetingId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ZoomPassword",
                table: "Events");
        }
    }
}
