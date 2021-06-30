using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class RemoveRequiredUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ABKC_Dogs_Breeds_BreedId",
                table: "ABKC_Dogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ABKC_Dogs_Owners_OwnerId",
                table: "ABKC_Dogs");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrationStatus_ABKCUsers_StatusChangedById",
                table: "RegistrationStatus");

            migrationBuilder.AlterColumn<int>(
                name: "StatusChangedById",
                table: "RegistrationStatus",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "ABKC_Dogs",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "BreedId",
                table: "ABKC_Dogs",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_ABKC_Dogs_Breeds_BreedId",
                table: "ABKC_Dogs",
                column: "BreedId",
                principalTable: "Breeds",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ABKC_Dogs_Owners_OwnerId",
                table: "ABKC_Dogs",
                column: "OwnerId",
                principalTable: "Owners",
                principalColumn: "Owner_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrationStatus_ABKCUsers_StatusChangedById",
                table: "RegistrationStatus",
                column: "StatusChangedById",
                principalTable: "ABKCUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ABKC_Dogs_Breeds_BreedId",
                table: "ABKC_Dogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ABKC_Dogs_Owners_OwnerId",
                table: "ABKC_Dogs");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrationStatus_ABKCUsers_StatusChangedById",
                table: "RegistrationStatus");

            migrationBuilder.AlterColumn<int>(
                name: "StatusChangedById",
                table: "RegistrationStatus",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "ABKC_Dogs",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BreedId",
                table: "ABKC_Dogs",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ABKC_Dogs_Breeds_BreedId",
                table: "ABKC_Dogs",
                column: "BreedId",
                principalTable: "Breeds",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ABKC_Dogs_Owners_OwnerId",
                table: "ABKC_Dogs",
                column: "OwnerId",
                principalTable: "Owners",
                principalColumn: "Owner_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrationStatus_ABKCUsers_StatusChangedById",
                table: "RegistrationStatus",
                column: "StatusChangedById",
                principalTable: "ABKCUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
