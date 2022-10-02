using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Laraue.Crawling.Dynamic.Tests.Migrations
{
    public partial class Initial2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MinutesToStop",
                table: "CianPages",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PublicTransportStop",
                table: "CianPages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TransportDistanceType",
                table: "CianPages",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinutesToStop",
                table: "CianPages");

            migrationBuilder.DropColumn(
                name: "PublicTransportStop",
                table: "CianPages");

            migrationBuilder.DropColumn(
                name: "TransportDistanceType",
                table: "CianPages");
        }
    }
}
