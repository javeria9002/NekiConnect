using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NekiConnect.Migrations
{
    /// <inheritdoc />
    public partial class AddEventIdToVolunteerApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CampaignId",
                table: "VolunteerApplications",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "VolunteerApplications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerApplications_EventId",
                table: "VolunteerApplications",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_VolunteerApplications_Events_EventId",
                table: "VolunteerApplications",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VolunteerApplications_Events_EventId",
                table: "VolunteerApplications");

            migrationBuilder.DropIndex(
                name: "IX_VolunteerApplications_EventId",
                table: "VolunteerApplications");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "VolunteerApplications");

            migrationBuilder.AlterColumn<int>(
                name: "CampaignId",
                table: "VolunteerApplications",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}