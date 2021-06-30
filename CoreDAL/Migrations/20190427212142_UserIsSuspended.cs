using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class UserIsSuspended : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSuspended",
                table: "ABKCUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSuspended",
                table: "ABKCUsers");
        }
    }
}
