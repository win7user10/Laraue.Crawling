using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Laraue.Crawling.Dynamic.Tests.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CianPages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TotalPrice = table.Column<int>(type: "integer", nullable: false),
                    OneMeterPrice = table.Column<int>(type: "integer", nullable: false),
                    RoomsCount = table.Column<int>(type: "integer", nullable: false),
                    Square = table.Column<decimal>(type: "numeric", nullable: false),
                    FloorNumber = table.Column<int>(type: "integer", nullable: false),
                    TotalFloorsNumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CianPages", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CianPages");
        }
    }
}
