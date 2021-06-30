using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class bullyIdRequests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BullyIdRequestModelId",
                table: "RegistrationStatus",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BullyIdRequests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmittedById = table.Column<int>(nullable: true),
                    SubmissionNotes = table.Column<string>(nullable: true),
                    IsInternationalRegistration = table.Column<bool>(nullable: false),
                    RushRequested = table.Column<bool>(nullable: false),
                    OvernightRequested = table.Column<bool>(nullable: false),
                    DogInfoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BullyIdRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BullyIdRequests_ABKC_Dogs_DogInfoId",
                        column: x => x.DogInfoId,
                        principalTable: "ABKC_Dogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BullyIdRequests_ABKCUsers_SubmittedById",
                        column: x => x.SubmittedById,
                        principalTable: "ABKCUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationStatus_BullyIdRequestModelId",
                table: "RegistrationStatus",
                column: "BullyIdRequestModelId");

            migrationBuilder.CreateIndex(
                name: "IX_BullyIdRequests_DogInfoId",
                table: "BullyIdRequests",
                column: "DogInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_BullyIdRequests_SubmittedById",
                table: "BullyIdRequests",
                column: "SubmittedById");

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrationStatus_BullyIdRequests_BullyIdRequestModelId",
                table: "RegistrationStatus",
                column: "BullyIdRequestModelId",
                principalTable: "BullyIdRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegistrationStatus_BullyIdRequests_BullyIdRequestModelId",
                table: "RegistrationStatus");

            migrationBuilder.DropTable(
                name: "BullyIdRequests");

            migrationBuilder.DropIndex(
                name: "IX_RegistrationStatus_BullyIdRequestModelId",
                table: "RegistrationStatus");

            migrationBuilder.DropColumn(
                name: "BullyIdRequestModelId",
                table: "RegistrationStatus");
        }
    }
}
