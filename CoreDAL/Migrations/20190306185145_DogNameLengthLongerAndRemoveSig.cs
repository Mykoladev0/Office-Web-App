using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class DogNameLengthLongerAndRemoveSig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoOwnerSignature",
                table: "ABKC_Dogs");

            migrationBuilder.DropColumn(
                name: "OwnerSignature",
                table: "ABKC_Dogs");

            migrationBuilder.AlterColumn<string>(
                name: "DogName",
                table: "ABKC_Dogs",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 35);

            migrationBuilder.AddColumn<int>(
                name: "CoOwnerSignatureId",
                table: "ABKC_Dogs",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OwnerSignatureId",
                table: "ABKC_Dogs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ABKC_Dogs_CoOwnerSignatureId",
                table: "ABKC_Dogs",
                column: "CoOwnerSignatureId");

            migrationBuilder.CreateIndex(
                name: "IX_ABKC_Dogs_OwnerSignatureId",
                table: "ABKC_Dogs",
                column: "OwnerSignatureId");

            migrationBuilder.AddForeignKey(
                name: "FK_ABKC_Dogs_Attachments_CoOwnerSignatureId",
                table: "ABKC_Dogs",
                column: "CoOwnerSignatureId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ABKC_Dogs_Attachments_OwnerSignatureId",
                table: "ABKC_Dogs",
                column: "OwnerSignatureId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ABKC_Dogs_Attachments_CoOwnerSignatureId",
                table: "ABKC_Dogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ABKC_Dogs_Attachments_OwnerSignatureId",
                table: "ABKC_Dogs");

            migrationBuilder.DropIndex(
                name: "IX_ABKC_Dogs_CoOwnerSignatureId",
                table: "ABKC_Dogs");

            migrationBuilder.DropIndex(
                name: "IX_ABKC_Dogs_OwnerSignatureId",
                table: "ABKC_Dogs");

            migrationBuilder.DropColumn(
                name: "CoOwnerSignatureId",
                table: "ABKC_Dogs");

            migrationBuilder.DropColumn(
                name: "OwnerSignatureId",
                table: "ABKC_Dogs");

            migrationBuilder.AlterColumn<string>(
                name: "DogName",
                table: "ABKC_Dogs",
                maxLength: 35,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AddColumn<byte[]>(
                name: "CoOwnerSignature",
                table: "ABKC_Dogs",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "OwnerSignature",
                table: "ABKC_Dogs",
                nullable: true);
        }
    }
}
