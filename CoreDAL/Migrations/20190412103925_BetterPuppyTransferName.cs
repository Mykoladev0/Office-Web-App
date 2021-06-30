using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class BetterPuppyTransferName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegistrationStatus_JrHandlerRegistrations_JrHandlerRegistrationId",
                table: "RegistrationStatus");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_ABKCUsers_UserModelId",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "IsPuppyTransfer",
                table: "PuppyRegistrations");

            migrationBuilder.AddColumn<bool>(
                name: "IsTransferRequest",
                table: "PuppyRegistrations",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrationStatus_JrHandlerRegistrations_JrHandlerRegistrationId",
                table: "RegistrationStatus",
                column: "JrHandlerRegistrationId",
                principalTable: "JrHandlerRegistrations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_ABKCUsers_UserModelId",
                table: "UserRoles",
                column: "UserModelId",
                principalTable: "ABKCUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegistrationStatus_JrHandlerRegistrations_JrHandlerRegistrationId",
                table: "RegistrationStatus");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_ABKCUsers_UserModelId",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "IsTransferRequest",
                table: "PuppyRegistrations");

            migrationBuilder.AddColumn<bool>(
                name: "IsPuppyTransfer",
                table: "PuppyRegistrations",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrationStatus_JrHandlerRegistrations_JrHandlerRegistrationId",
                table: "RegistrationStatus",
                column: "JrHandlerRegistrationId",
                principalTable: "JrHandlerRegistrations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_ABKCUsers_UserModelId",
                table: "UserRoles",
                column: "UserModelId",
                principalTable: "ABKCUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
