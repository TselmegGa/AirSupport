using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AirSupport.Application.PassengerManagement.Migrations
{
    public partial class ChangedIntoEventStore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Flights",
                table: "Flights");

            migrationBuilder.AlterColumn<int>(
                name: "FlightId",
                table: "Flights",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeStamp",
                table: "Flights",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ArrivalTime",
                table: "Flights",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Counter",
                table: "Flights",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Flights",
                table: "Flights",
                columns: new[] { "FlightId", "TimeStamp" });

            migrationBuilder.CreateTable(
                name: "FlightEvent",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FlightId = table.Column<int>(nullable: false),
                    MessageType = table.Column<string>(nullable: true),
                    EventData = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlightEvent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Flights",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "TimeStamp",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "ArrivalTime",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "Counter",
                table: "Flights");

            migrationBuilder.AlterColumn<string>(
                name: "FlightId",
                table: "Flights",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Flights",
                table: "Flights",
                column: "FlightId");
        }
    }
}
