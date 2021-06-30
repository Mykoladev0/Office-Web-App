using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class PuppyRegistration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PuppyRegistrationId",
                table: "RegistrationStatus",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PuppyRegistrations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    SubmittedById = table.Column<int>(nullable: true),
                    SubmissionNotes = table.Column<string>(nullable: true),
                    IsInternationalRegistration = table.Column<bool>(nullable: false),
                    RushRequested = table.Column<bool>(nullable: false),
                    OvernightRequested = table.Column<bool>(nullable: false),
                    OwnerSignature = table.Column<byte[]>(nullable: true),
                    CoOwnerSignature = table.Column<byte[]>(nullable: true),
                    DogInfoId = table.Column<int>(nullable: true),
                    NewOwnerOwnerId = table.Column<int>(nullable: true),
                    NewCoOwnerOwnerId = table.Column<int>(nullable: true),
                    BothSignaturesRequiredWhenRegistering = table.Column<bool>(nullable: false),
                    DateOfSale = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PuppyRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PuppyRegistrations_ABKC_Dogs_DogInfoId",
                        column: x => x.DogInfoId,
                        principalTable: "ABKC_Dogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PuppyRegistrations_Owners_NewCoOwnerOwnerId",
                        column: x => x.NewCoOwnerOwnerId,
                        principalTable: "Owners",
                        principalColumn: "Owner_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PuppyRegistrations_Owners_NewOwnerOwnerId",
                        column: x => x.NewOwnerOwnerId,
                        principalTable: "Owners",
                        principalColumn: "Owner_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PuppyRegistrations_ABKCUsers_SubmittedById",
                        column: x => x.SubmittedById,
                        principalTable: "ABKCUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationStatus_PuppyRegistrationId",
                table: "RegistrationStatus",
                column: "PuppyRegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_PuppyRegistrations_DogInfoId",
                table: "PuppyRegistrations",
                column: "DogInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_PuppyRegistrations_Id",
                table: "PuppyRegistrations",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PuppyRegistrations_NewCoOwnerOwnerId",
                table: "PuppyRegistrations",
                column: "NewCoOwnerOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PuppyRegistrations_NewOwnerOwnerId",
                table: "PuppyRegistrations",
                column: "NewOwnerOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PuppyRegistrations_SubmittedById",
                table: "PuppyRegistrations",
                column: "SubmittedById");

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrationStatus_PuppyRegistrations_PuppyRegistrationId",
                table: "RegistrationStatus",
                column: "PuppyRegistrationId",
                principalTable: "PuppyRegistrations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegistrationStatus_PuppyRegistrations_PuppyRegistrationId",
                table: "RegistrationStatus");

            migrationBuilder.DropTable(
                name: "PuppyRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_RegistrationStatus_PuppyRegistrationId",
                table: "RegistrationStatus");

            migrationBuilder.DropColumn(
                name: "PuppyRegistrationId",
                table: "RegistrationStatus");
        }
    }
}
