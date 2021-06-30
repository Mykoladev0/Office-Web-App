using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class ABKCDogsUsersAndRegistrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ABKC_Dogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    DogName = table.Column<string>(maxLength: 35, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MicrochipNumber = table.Column<string>(nullable: true),
                    Gender = table.Column<int>(nullable: false),
                    BreedId = table.Column<int>(nullable: false),
                    OwnerId = table.Column<int>(nullable: false),
                    CoOwnerOwnerId = table.Column<int>(nullable: true),
                    OwnerSignature = table.Column<byte[]>(nullable: true),
                    CoOwnerSignature = table.Column<byte[]>(nullable: true),
                    DamId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ABKC_Dogs", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_ABKC_Dogs_Breeds_BreedId",
                        column: x => x.BreedId,
                        principalTable: "Breeds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ABKC_Dogs_Owners_CoOwnerOwnerId",
                        column: x => x.CoOwnerOwnerId,
                        principalTable: "Owners",
                        principalColumn: "Owner_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ABKC_Dogs_ABKC_Dogs_DamId",
                        column: x => x.DamId,
                        principalTable: "ABKC_Dogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ABKC_Dogs_Owners_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Owners",
                        principalColumn: "Owner_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ABKCUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    OktaId = table.Column<string>(maxLength: 80, nullable: false),
                    LoginName = table.Column<string>(maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ABKCUsers", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "DogColorTable",
                columns: table => new
                {
                    DogId = table.Column<int>(nullable: false),
                    ColorId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DogColorTable", x => new { x.DogId, x.ColorId });
                    table.ForeignKey(
                        name: "FK_DogColorTable_Colors_ColorId",
                        column: x => x.ColorId,
                        principalTable: "Colors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DogColorTable_ABKC_Dogs_DogId",
                        column: x => x.DogId,
                        principalTable: "ABKC_Dogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Registrations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    SubmittedById = table.Column<int>(nullable: true),
                    IsInternationalRegistration = table.Column<bool>(nullable: false),
                    RushRequested = table.Column<bool>(nullable: false),
                    OvernightRequested = table.Column<bool>(nullable: false),
                    OwnerSignature = table.Column<byte[]>(nullable: true),
                    CoOwnerSignature = table.Column<byte[]>(nullable: true),
                    DogId = table.Column<int>(nullable: false),
                    SubmissionNotes = table.Column<string>(nullable: true),
                    FrontPedigree = table.Column<byte[]>(nullable: true),
                    BackPedigree = table.Column<byte[]>(nullable: true),
                    FrontPhoto = table.Column<byte[]>(nullable: true),
                    SidePhoto = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registrations", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_Registrations_ABKC_Dogs_DogId",
                        column: x => x.DogId,
                        principalTable: "ABKC_Dogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Registrations_ABKCUsers_SubmittedById",
                        column: x => x.SubmittedById,
                        principalTable: "ABKCUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationStatus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    Status = table.Column<int>(nullable: false),
                    StatusChangedById = table.Column<int>(nullable: false),
                    Comments = table.Column<string>(nullable: true),
                    RegistrationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationStatus", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_RegistrationStatus_Registrations_RegistrationId",
                        column: x => x.RegistrationId,
                        principalTable: "Registrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegistrationStatus_ABKCUsers_StatusChangedById",
                        column: x => x.StatusChangedById,
                        principalTable: "ABKCUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ABKC_Dogs_BreedId",
                table: "ABKC_Dogs",
                column: "BreedId");

            migrationBuilder.CreateIndex(
                name: "IX_ABKC_Dogs_CoOwnerOwnerId",
                table: "ABKC_Dogs",
                column: "CoOwnerOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ABKC_Dogs_DamId",
                table: "ABKC_Dogs",
                column: "DamId",
                unique: true,
                filter: "[DamId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ABKC_Dogs_Id",
                table: "ABKC_Dogs",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ABKC_Dogs_OwnerId",
                table: "ABKC_Dogs",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ABKCUsers_Id",
                table: "ABKCUsers",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DogColorTable_ColorId",
                table: "DogColorTable",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_DogId",
                table: "Registrations",
                column: "DogId");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_Id",
                table: "Registrations",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_SubmittedById",
                table: "Registrations",
                column: "SubmittedById");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationStatus_Id",
                table: "RegistrationStatus",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationStatus_RegistrationId",
                table: "RegistrationStatus",
                column: "RegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationStatus_StatusChangedById",
                table: "RegistrationStatus",
                column: "StatusChangedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DogColorTable");

            migrationBuilder.DropTable(
                name: "RegistrationStatus");

            migrationBuilder.DropTable(
                name: "Registrations");

            migrationBuilder.DropTable(
                name: "ABKC_Dogs");

            migrationBuilder.DropTable(
                name: "ABKCUsers");
        }
    }
}
