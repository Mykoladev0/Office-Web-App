using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class AddSireReferenceToABKCDogTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ABKC_Dogs_Owners_CoOwnerOwnerId",
                table: "ABKC_Dogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_ABKC_Dogs_DogId",
                table: "Registrations");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrationStatus_JrHandlerRegistrations_JrHandlerRegistrationId",
                table: "RegistrationStatus");

            migrationBuilder.DropIndex(
                name: "IX_ABKC_Dogs_DamId",
                table: "ABKC_Dogs");

            migrationBuilder.RenameColumn(
                name: "CoOwnerOwnerId",
                table: "ABKC_Dogs",
                newName: "SireId");

            migrationBuilder.RenameIndex(
                name: "IX_ABKC_Dogs_CoOwnerOwnerId",
                table: "ABKC_Dogs",
                newName: "IX_ABKC_Dogs_SireId");

            migrationBuilder.AlterColumn<int>(
                name: "NewCoOwnerId",
                table: "PuppyRegistrations",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "CoOwnerId",
                table: "ABKC_Dogs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ABKC_Dogs_CoOwnerId",
                table: "ABKC_Dogs",
                column: "CoOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ABKC_Dogs_DamId",
                table: "ABKC_Dogs",
                column: "DamId");

            migrationBuilder.AddForeignKey(
                name: "FK_ABKC_Dogs_Owners_CoOwnerId",
                table: "ABKC_Dogs",
                column: "CoOwnerId",
                principalTable: "Owners",
                principalColumn: "Owner_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ABKC_Dogs_ABKC_Dogs_SireId",
                table: "ABKC_Dogs",
                column: "SireId",
                principalTable: "ABKC_Dogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_ABKC_Dogs_DogId",
                table: "Registrations",
                column: "DogId",
                principalTable: "ABKC_Dogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrationStatus_JrHandlerRegistrations_JrHandlerRegistrationId",
                table: "RegistrationStatus",
                column: "JrHandlerRegistrationId",
                principalTable: "JrHandlerRegistrations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ABKC_Dogs_Owners_CoOwnerId",
                table: "ABKC_Dogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ABKC_Dogs_ABKC_Dogs_SireId",
                table: "ABKC_Dogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_ABKC_Dogs_DogId",
                table: "Registrations");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrationStatus_JrHandlerRegistrations_JrHandlerRegistrationId",
                table: "RegistrationStatus");

            migrationBuilder.DropIndex(
                name: "IX_ABKC_Dogs_CoOwnerId",
                table: "ABKC_Dogs");

            migrationBuilder.DropIndex(
                name: "IX_ABKC_Dogs_DamId",
                table: "ABKC_Dogs");

            migrationBuilder.DropColumn(
                name: "CoOwnerId",
                table: "ABKC_Dogs");

            migrationBuilder.RenameColumn(
                name: "SireId",
                table: "ABKC_Dogs",
                newName: "CoOwnerOwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_ABKC_Dogs_SireId",
                table: "ABKC_Dogs",
                newName: "IX_ABKC_Dogs_CoOwnerOwnerId");

            migrationBuilder.AlterColumn<int>(
                name: "NewCoOwnerId",
                table: "PuppyRegistrations",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ABKC_Dogs_DamId",
                table: "ABKC_Dogs",
                column: "DamId",
                unique: true,
                filter: "[DamId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ABKC_Dogs_Owners_CoOwnerOwnerId",
                table: "ABKC_Dogs",
                column: "CoOwnerOwnerId",
                principalTable: "Owners",
                principalColumn: "Owner_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_ABKC_Dogs_DogId",
                table: "Registrations",
                column: "DogId",
                principalTable: "ABKC_Dogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrationStatus_JrHandlerRegistrations_JrHandlerRegistrationId",
                table: "RegistrationStatus",
                column: "JrHandlerRegistrationId",
                principalTable: "JrHandlerRegistrations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
