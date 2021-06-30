using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class LastModifiedToABKCDogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LastModifiedById",
                table: "ABKC_Dogs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ABKC_Dogs_LastModifiedById",
                table: "ABKC_Dogs",
                column: "LastModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_ABKC_Dogs_ABKCUsers_LastModifiedById",
                table: "ABKC_Dogs",
                column: "LastModifiedById",
                principalTable: "ABKCUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ABKC_Dogs_ABKCUsers_LastModifiedById",
                table: "ABKC_Dogs");

            migrationBuilder.DropIndex(
                name: "IX_ABKC_Dogs_LastModifiedById",
                table: "ABKC_Dogs");

            migrationBuilder.DropColumn(
                name: "LastModifiedById",
                table: "ABKC_Dogs");
        }
    }
}
