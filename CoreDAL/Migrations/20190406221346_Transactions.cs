using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class Transactions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TransactionId",
                table: "Registrations",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StripeChargeId = table.Column<string>(nullable: true),
                    ChargedToId = table.Column<int>(nullable: true),
                    Amount = table.Column<double>(nullable: false),
                    RegistrationsAsJSON = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_ABKCUsers_ChargedToId",
                        column: x => x.ChargedToId,
                        principalTable: "ABKCUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_TransactionId",
                table: "Registrations",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ChargedToId",
                table: "Transactions",
                column: "ChargedToId");

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_Transactions_TransactionId",
                table: "Registrations",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_Transactions_TransactionId",
                table: "Registrations");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Registrations_TransactionId",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Registrations");
        }
    }
}
