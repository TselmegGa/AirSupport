using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AirSupport.Application.PassengerManagement.Migrations
{
    public partial class updateDB21520212 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ArrivalDate",
                table: "Flights",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ArrivalDate",
                table: "Flights",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
