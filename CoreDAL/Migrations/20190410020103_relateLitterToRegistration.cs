using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class relateLitterToRegistration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LitterFromRegistrationId",
                table: "LitterRegistrations",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LitterRegistrations_LitterFromRegistrationId",
                table: "LitterRegistrations",
                column: "LitterFromRegistrationId");

            migrationBuilder.AddForeignKey(
                name: "FK_LitterRegistrations_Litters_LitterFromRegistrationId",
                table: "LitterRegistrations",
                column: "LitterFromRegistrationId",
                principalTable: "Litters",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LitterRegistrations_Litters_LitterFromRegistrationId",
                table: "LitterRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_LitterRegistrations_LitterFromRegistrationId",
                table: "LitterRegistrations");

            migrationBuilder.DropColumn(
                name: "LitterFromRegistrationId",
                table: "LitterRegistrations");
        }
    }
}
