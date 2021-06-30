using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class MoveAttachmentsToSeparateTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackPedigree",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "CoOwnerSignature",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "FrontPedigree",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "FrontPhoto",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "OwnerSignature",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "SidePhoto",
                table: "Registrations");

            migrationBuilder.AddColumn<int>(
                name: "BackPedigreeId",
                table: "Registrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CoOwnerSignatureId",
                table: "Registrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FrontPedigreeId",
                table: "Registrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FrontPhotoId",
                table: "Registrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OwnerSignatureId",
                table: "Registrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SidePhotoId",
                table: "Registrations",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Attachments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    FileName = table.Column<string>(nullable: true),
                    Data = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_BackPedigreeId",
                table: "Registrations",
                column: "BackPedigreeId");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_CoOwnerSignatureId",
                table: "Registrations",
                column: "CoOwnerSignatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_FrontPedigreeId",
                table: "Registrations",
                column: "FrontPedigreeId");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_FrontPhotoId",
                table: "Registrations",
                column: "FrontPhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_OwnerSignatureId",
                table: "Registrations",
                column: "OwnerSignatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_SidePhotoId",
                table: "Registrations",
                column: "SidePhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_Id",
                table: "Attachments",
                column: "Id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_Attachments_BackPedigreeId",
                table: "Registrations",
                column: "BackPedigreeId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_Attachments_CoOwnerSignatureId",
                table: "Registrations",
                column: "CoOwnerSignatureId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_Attachments_FrontPedigreeId",
                table: "Registrations",
                column: "FrontPedigreeId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_Attachments_FrontPhotoId",
                table: "Registrations",
                column: "FrontPhotoId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_Attachments_OwnerSignatureId",
                table: "Registrations",
                column: "OwnerSignatureId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_Attachments_SidePhotoId",
                table: "Registrations",
                column: "SidePhotoId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_Attachments_BackPedigreeId",
                table: "Registrations");

            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_Attachments_CoOwnerSignatureId",
                table: "Registrations");

            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_Attachments_FrontPedigreeId",
                table: "Registrations");

            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_Attachments_FrontPhotoId",
                table: "Registrations");

            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_Attachments_OwnerSignatureId",
                table: "Registrations");

            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_Attachments_SidePhotoId",
                table: "Registrations");

            migrationBuilder.DropTable(
                name: "Attachments");

            migrationBuilder.DropIndex(
                name: "IX_Registrations_BackPedigreeId",
                table: "Registrations");

            migrationBuilder.DropIndex(
                name: "IX_Registrations_CoOwnerSignatureId",
                table: "Registrations");

            migrationBuilder.DropIndex(
                name: "IX_Registrations_FrontPedigreeId",
                table: "Registrations");

            migrationBuilder.DropIndex(
                name: "IX_Registrations_FrontPhotoId",
                table: "Registrations");

            migrationBuilder.DropIndex(
                name: "IX_Registrations_OwnerSignatureId",
                table: "Registrations");

            migrationBuilder.DropIndex(
                name: "IX_Registrations_SidePhotoId",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "BackPedigreeId",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "CoOwnerSignatureId",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "FrontPedigreeId",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "FrontPhotoId",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "OwnerSignatureId",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "SidePhotoId",
                table: "Registrations");

            migrationBuilder.AddColumn<byte[]>(
                name: "BackPedigree",
                table: "Registrations",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "CoOwnerSignature",
                table: "Registrations",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "FrontPedigree",
                table: "Registrations",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "FrontPhoto",
                table: "Registrations",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "OwnerSignature",
                table: "Registrations",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "SidePhoto",
                table: "Registrations",
                nullable: true);
        }
    }
}
