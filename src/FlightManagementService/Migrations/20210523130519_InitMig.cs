using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AirSupport.Application.PassengerManagement.Migrations
{
    public partial class InitMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Flights",
                columns: table => new
                {
                    FlightId = table.Column<string>(nullable: false),
                    Gate = table.Column<string>(nullable: true),
                    Origin = table.Column<string>(nullable: true),
                    Destination = table.Column<string>(nullable: true),
                    DepartureTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flights", x => x.FlightId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Flights");
        }
    }
}
