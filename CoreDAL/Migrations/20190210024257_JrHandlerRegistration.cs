using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class JrHandlerRegistration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegistrationStatus_Registrations_RegistrationId",
                table: "RegistrationStatus");

            migrationBuilder.DropIndex(
                name: "IX_RegistrationStatus_RegistrationId",
                table: "RegistrationStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Registrations",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "RegistrationId",
                table: "RegistrationStatus");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "RegistrationStatus",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DogRegistrationId",
                table: "RegistrationStatus",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JrHandlerRegistrationId",
                table: "RegistrationStatus",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Registrations",
                table: "Registrations",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "JrHandlerRegistrations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    SubmittedById = table.Column<int>(nullable: true),
                    SubmissionNotes = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    DateOfBirth = table.Column<DateTime>(nullable: true),
                    ParentLastName = table.Column<string>(nullable: true),
                    ParentFirstName = table.Column<string>(nullable: true),
                    Address1 = table.Column<string>(nullable: true),
                    Address2 = table.Column<string>(nullable: true),
                    Address3 = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    Zip = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    International = table.Column<bool>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Cell = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JrHandlerRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JrHandlerRegistrations_ABKCUsers_SubmittedById",
                        column: x => x.SubmittedById,
                        principalTable: "ABKCUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationStatus_DogRegistrationId",
                table: "RegistrationStatus",
                column: "DogRegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationStatus_JrHandlerRegistrationId",
                table: "RegistrationStatus",
                column: "JrHandlerRegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_JrHandlerRegistrations_Id",
                table: "JrHandlerRegistrations",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JrHandlerRegistrations_SubmittedById",
                table: "JrHandlerRegistrations",
                column: "SubmittedById");

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrationStatus_Registrations_DogRegistrationId",
                table: "RegistrationStatus",
                column: "DogRegistrationId",
                principalTable: "Registrations",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegistrationStatus_Registrations_DogRegistrationId",
                table: "RegistrationStatus");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrationStatus_JrHandlerRegistrations_JrHandlerRegistrationId",
                table: "RegistrationStatus");

            migrationBuilder.DropTable(
                name: "JrHandlerRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_RegistrationStatus_DogRegistrationId",
                table: "RegistrationStatus");

            migrationBuilder.DropIndex(
                name: "IX_RegistrationStatus_JrHandlerRegistrationId",
                table: "RegistrationStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Registrations",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "RegistrationStatus");

            migrationBuilder.DropColumn(
                name: "DogRegistrationId",
                table: "RegistrationStatus");

            migrationBuilder.DropColumn(
                name: "JrHandlerRegistrationId",
                table: "RegistrationStatus");

            migrationBuilder.AddColumn<int>(
                name: "RegistrationId",
                table: "RegistrationStatus",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Registrations",
                table: "Registrations",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationStatus_RegistrationId",
                table: "RegistrationStatus",
                column: "RegistrationId");

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrationStatus_Registrations_RegistrationId",
                table: "RegistrationStatus",
                column: "RegistrationId",
                principalTable: "Registrations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
