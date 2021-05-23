using Microsoft.EntityFrameworkCore.Migrations;

namespace AirSupport.Application.PassengerManagement.Migrations
{
    public partial class update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Flights",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");
            migrationBuilder.Sql("SET IDENTITY_INSERT FLights ON");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Flights",
                type: "int",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("SqlServer:Identity", "1, 1");
             migrationBuilder.Sql("SET IDENTITY_INSERT FLights OFF");
        }
    }
}
