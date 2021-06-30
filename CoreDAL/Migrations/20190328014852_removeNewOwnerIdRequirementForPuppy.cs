using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class removeNewOwnerIdRequirementForPuppy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "NewOwnerId",
                table: "PuppyRegistrations",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "NewOwnerId",
                table: "PuppyRegistrations",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
