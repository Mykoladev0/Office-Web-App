using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class AddOriginalIdToNewDogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OriginalDogTableId",
                table: "ABKC_Dogs",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalDogTableId",
                table: "ABKC_Dogs");
        }
    }
}
