using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class litterRegistration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LitterRegistrationId",
                table: "RegistrationStatus",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LitterRegistrations",
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
                    BreedId = table.Column<int>(nullable: true),
                    SireId = table.Column<int>(nullable: false),
                    DamId = table.Column<int>(nullable: true),
                    DateOfBreeding = table.Column<DateTime>(nullable: true),
                    DateOfLitterBirth = table.Column<DateTime>(nullable: true),
                    NumberOfMalesBeingRegistered = table.Column<int>(nullable: false),
                    NumberOfFemalesBeingRegistered = table.Column<int>(nullable: false),
                    FrozenSemenUsed = table.Column<bool>(nullable: false),
                    DateSemenCollected = table.Column<DateTime>(nullable: true),
                    SireOwnerSignatureId = table.Column<int>(nullable: true),
                    SireCoOwnerSignatureId = table.Column<int>(nullable: true),
                    DamSignatureId = table.Column<int>(nullable: true),
                    DamCoOwnerSignatureId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LitterRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LitterRegistrations_Breeds_BreedId",
                        column: x => x.BreedId,
                        principalTable: "Breeds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LitterRegistrations_Attachments_DamCoOwnerSignatureId",
                        column: x => x.DamCoOwnerSignatureId,
                        principalTable: "Attachments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LitterRegistrations_ABKC_Dogs_DamId",
                        column: x => x.DamId,
                        principalTable: "ABKC_Dogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LitterRegistrations_Attachments_DamSignatureId",
                        column: x => x.DamSignatureId,
                        principalTable: "Attachments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LitterRegistrations_Attachments_SireCoOwnerSignatureId",
                        column: x => x.SireCoOwnerSignatureId,
                        principalTable: "Attachments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LitterRegistrations_ABKC_Dogs_SireId",
                        column: x => x.SireId,
                        principalTable: "ABKC_Dogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LitterRegistrations_Attachments_SireOwnerSignatureId",
                        column: x => x.SireOwnerSignatureId,
                        principalTable: "Attachments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LitterRegistrations_ABKCUsers_SubmittedById",
                        column: x => x.SubmittedById,
                        principalTable: "ABKCUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationStatus_LitterRegistrationId",
                table: "RegistrationStatus",
                column: "LitterRegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_LitterRegistrations_BreedId",
                table: "LitterRegistrations",
                column: "BreedId");

            migrationBuilder.CreateIndex(
                name: "IX_LitterRegistrations_DamCoOwnerSignatureId",
                table: "LitterRegistrations",
                column: "DamCoOwnerSignatureId");

            migrationBuilder.CreateIndex(
                name: "IX_LitterRegistrations_DamId",
                table: "LitterRegistrations",
                column: "DamId");

            migrationBuilder.CreateIndex(
                name: "IX_LitterRegistrations_DamSignatureId",
                table: "LitterRegistrations",
                column: "DamSignatureId");

            migrationBuilder.CreateIndex(
                name: "IX_LitterRegistrations_Id",
                table: "LitterRegistrations",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LitterRegistrations_SireCoOwnerSignatureId",
                table: "LitterRegistrations",
                column: "SireCoOwnerSignatureId");

            migrationBuilder.CreateIndex(
                name: "IX_LitterRegistrations_SireId",
                table: "LitterRegistrations",
                column: "SireId");

            migrationBuilder.CreateIndex(
                name: "IX_LitterRegistrations_SireOwnerSignatureId",
                table: "LitterRegistrations",
                column: "SireOwnerSignatureId");

            migrationBuilder.CreateIndex(
                name: "IX_LitterRegistrations_SubmittedById",
                table: "LitterRegistrations",
                column: "SubmittedById");

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrationStatus_LitterRegistrations_LitterRegistrationId",
                table: "RegistrationStatus",
                column: "LitterRegistrationId",
                principalTable: "LitterRegistrations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegistrationStatus_LitterRegistrations_LitterRegistrationId",
                table: "RegistrationStatus");

            migrationBuilder.DropTable(
                name: "LitterRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_RegistrationStatus_LitterRegistrationId",
                table: "RegistrationStatus");

            migrationBuilder.DropColumn(
                name: "LitterRegistrationId",
                table: "RegistrationStatus");
        }
    }
}
