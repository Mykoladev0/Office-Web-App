using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class AddLitterAndABKCToDogsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ABKCNumber",
                table: "ABKC_Dogs",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LitterId",
                table: "ABKC_Dogs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ABKC_Dogs_LitterId",
                table: "ABKC_Dogs",
                column: "LitterId");

            migrationBuilder.AddForeignKey(
                name: "FK_ABKC_Dogs_Litters_LitterId",
                table: "ABKC_Dogs",
                column: "LitterId",
                principalTable: "Litters",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ABKC_Dogs_Litters_LitterId",
                table: "ABKC_Dogs");

            migrationBuilder.DropIndex(
                name: "IX_ABKC_Dogs_LitterId",
                table: "ABKC_Dogs");

            migrationBuilder.DropColumn(
                name: "ABKCNumber",
                table: "ABKC_Dogs");

            migrationBuilder.DropColumn(
                name: "LitterId",
                table: "ABKC_Dogs");
        }
    }
}
