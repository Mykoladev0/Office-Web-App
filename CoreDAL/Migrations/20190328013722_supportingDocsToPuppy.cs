using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class supportingDocsToPuppy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BothSignaturesRequiredWhenRegistering",
                table: "PuppyRegistrations");

            migrationBuilder.DropColumn(
                name: "CoOwnerSignature",
                table: "PuppyRegistrations");

            migrationBuilder.DropColumn(
                name: "OwnerSignature",
                table: "PuppyRegistrations");

            migrationBuilder.AddColumn<int>(
                name: "CoOwnerSignatureId",
                table: "PuppyRegistrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CoSellerSignatureId",
                table: "PuppyRegistrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OwnerSignatureId",
                table: "PuppyRegistrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SellerSignatureId",
                table: "PuppyRegistrations",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PuppyRegistrations_CoOwnerSignatureId",
                table: "PuppyRegistrations",
                column: "CoOwnerSignatureId");

            migrationBuilder.CreateIndex(
                name: "IX_PuppyRegistrations_CoSellerSignatureId",
                table: "PuppyRegistrations",
                column: "CoSellerSignatureId");

            migrationBuilder.CreateIndex(
                name: "IX_PuppyRegistrations_OwnerSignatureId",
                table: "PuppyRegistrations",
                column: "OwnerSignatureId");

            migrationBuilder.CreateIndex(
                name: "IX_PuppyRegistrations_SellerSignatureId",
                table: "PuppyRegistrations",
                column: "SellerSignatureId");

            migrationBuilder.AddForeignKey(
                name: "FK_PuppyRegistrations_Attachments_CoOwnerSignatureId",
                table: "PuppyRegistrations",
                column: "CoOwnerSignatureId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PuppyRegistrations_Attachments_CoSellerSignatureId",
                table: "PuppyRegistrations",
                column: "CoSellerSignatureId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PuppyRegistrations_Attachments_OwnerSignatureId",
                table: "PuppyRegistrations",
                column: "OwnerSignatureId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PuppyRegistrations_Attachments_SellerSignatureId",
                table: "PuppyRegistrations",
                column: "SellerSignatureId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PuppyRegistrations_Attachments_CoOwnerSignatureId",
                table: "PuppyRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_PuppyRegistrations_Attachments_CoSellerSignatureId",
                table: "PuppyRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_PuppyRegistrations_Attachments_OwnerSignatureId",
                table: "PuppyRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_PuppyRegistrations_Attachments_SellerSignatureId",
                table: "PuppyRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_PuppyRegistrations_CoOwnerSignatureId",
                table: "PuppyRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_PuppyRegistrations_CoSellerSignatureId",
                table: "PuppyRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_PuppyRegistrations_OwnerSignatureId",
                table: "PuppyRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_PuppyRegistrations_SellerSignatureId",
                table: "PuppyRegistrations");

            migrationBuilder.DropColumn(
                name: "CoOwnerSignatureId",
                table: "PuppyRegistrations");

            migrationBuilder.DropColumn(
                name: "CoSellerSignatureId",
                table: "PuppyRegistrations");

            migrationBuilder.DropColumn(
                name: "OwnerSignatureId",
                table: "PuppyRegistrations");

            migrationBuilder.DropColumn(
                name: "SellerSignatureId",
                table: "PuppyRegistrations");

            migrationBuilder.AddColumn<bool>(
                name: "BothSignaturesRequiredWhenRegistering",
                table: "PuppyRegistrations",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "CoOwnerSignature",
                table: "PuppyRegistrations",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "OwnerSignature",
                table: "PuppyRegistrations",
                nullable: true);
        }
    }
}
