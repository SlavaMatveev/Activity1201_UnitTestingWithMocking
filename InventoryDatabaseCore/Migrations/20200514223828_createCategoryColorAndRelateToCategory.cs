using Microsoft.EntityFrameworkCore.Migrations;

namespace InventoryDatabaseCore.Migrations
{
    public partial class createCategoryColorAndRelateToCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryColorId",
                table: "Categories",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CategoryColors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    ColorValue = table.Column<string>(maxLength: 7, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryColors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryColors_Categories_Id",
                        column: x => x.Id,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryColors");

            migrationBuilder.DropColumn(
                name: "CategoryColorId",
                table: "Categories");
        }
    }
}
