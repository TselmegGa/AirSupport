using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AirSupport.Application.PassengerManagement.Migrations
{
    public partial class SetPassengerProps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "Passengers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CellNumber",
                table: "Passengers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Passengers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Passengers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "FlightId",
                table: "Passengers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Passengers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Passengers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                table: "Passengers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Passengers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Flights",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Origin = table.Column<string>(nullable: false),
                    Destination = table.Column<string>(nullable: false),
                    ArrivalGate = table.Column<string>(nullable: false),
                    ArrivalDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flights", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_FlightId",
                table: "Passengers",
                column: "FlightId");

            migrationBuilder.AddForeignKey(
                name: "FK_Passengers_Flights_FlightId",
                table: "Passengers",
                column: "FlightId",
                principalTable: "Flights",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Passengers_Flights_FlightId",
                table: "Passengers");

            migrationBuilder.DropTable(
                name: "Flights");

            migrationBuilder.DropIndex(
                name: "IX_Passengers_FlightId",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "CellNumber",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "FlightId",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Passengers");
        }
    }
}
