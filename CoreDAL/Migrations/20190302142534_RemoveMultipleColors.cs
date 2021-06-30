using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class RemoveMultipleColors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DogColorTable");

            migrationBuilder.AddColumn<int>(
                name: "ColorId",
                table: "ABKC_Dogs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ABKC_Dogs_ColorId",
                table: "ABKC_Dogs",
                column: "ColorId");

            migrationBuilder.AddForeignKey(
                name: "FK_ABKC_Dogs_Colors_ColorId",
                table: "ABKC_Dogs",
                column: "ColorId",
                principalTable: "Colors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ABKC_Dogs_Colors_ColorId",
                table: "ABKC_Dogs");

            migrationBuilder.DropIndex(
                name: "IX_ABKC_Dogs_ColorId",
                table: "ABKC_Dogs");

            migrationBuilder.DropColumn(
                name: "ColorId",
                table: "ABKC_Dogs");

            migrationBuilder.CreateTable(
                name: "DogColorTable",
                columns: table => new
                {
                    DogId = table.Column<int>(nullable: false),
                    ColorId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DogColorTable", x => new { x.DogId, x.ColorId });
                    table.ForeignKey(
                        name: "FK_DogColorTable_Colors_ColorId",
                        column: x => x.ColorId,
                        principalTable: "Colors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DogColorTable_ABKC_Dogs_DogId",
                        column: x => x.DogId,
                        principalTable: "ABKC_Dogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DogColorTable_ColorId",
                table: "DogColorTable",
                column: "ColorId");
        }
    }
}
