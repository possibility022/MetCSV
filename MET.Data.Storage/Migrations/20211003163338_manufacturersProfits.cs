using Microsoft.EntityFrameworkCore.Migrations;

namespace MET.Data.Storage.Migrations
{
    public partial class manufacturersProfits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ManufacturerProfits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Manufacturer = table.Column<string>(type: "TEXT", nullable: true),
                    Provider = table.Column<int>(type: "INTEGER", nullable: false),
                    Profit = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManufacturerProfits", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManufacturerProfits");
        }
    }
}
