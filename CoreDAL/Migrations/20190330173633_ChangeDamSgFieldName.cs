using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class ChangeDamSgFieldName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LitterRegistrations_Attachments_DamSignatureId",
                table: "LitterRegistrations");

            migrationBuilder.RenameColumn(
                name: "DamSignatureId",
                table: "LitterRegistrations",
                newName: "DamOwnerSignatureId");

            migrationBuilder.RenameIndex(
                name: "IX_LitterRegistrations_DamSignatureId",
                table: "LitterRegistrations",
                newName: "IX_LitterRegistrations_DamOwnerSignatureId");

            migrationBuilder.AddForeignKey(
                name: "FK_LitterRegistrations_Attachments_DamOwnerSignatureId",
                table: "LitterRegistrations",
                column: "DamOwnerSignatureId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LitterRegistrations_Attachments_DamOwnerSignatureId",
                table: "LitterRegistrations");

            migrationBuilder.RenameColumn(
                name: "DamOwnerSignatureId",
                table: "LitterRegistrations",
                newName: "DamSignatureId");

            migrationBuilder.RenameIndex(
                name: "IX_LitterRegistrations_DamOwnerSignatureId",
                table: "LitterRegistrations",
                newName: "IX_LitterRegistrations_DamSignatureId");

            migrationBuilder.AddForeignKey(
                name: "FK_LitterRegistrations_Attachments_DamSignatureId",
                table: "LitterRegistrations",
                column: "DamSignatureId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
