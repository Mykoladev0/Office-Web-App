using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class refundtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Refunds",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RefundAmount = table.Column<double>(nullable: false),
                    IssuedById = table.Column<int>(nullable: true),
                    RefundedToId = table.Column<int>(nullable: true),
                    StripeID = table.Column<string>(nullable: true),
                    OriginalTransactionId = table.Column<int>(nullable: true),
                    RegistrationsRefundedAsJSON = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Refunds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Refunds_ABKCUsers_IssuedById",
                        column: x => x.IssuedById,
                        principalTable: "ABKCUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Refunds_Transactions_OriginalTransactionId",
                        column: x => x.OriginalTransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Refunds_ABKCUsers_RefundedToId",
                        column: x => x.RefundedToId,
                        principalTable: "ABKCUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_IssuedById",
                table: "Refunds",
                column: "IssuedById");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_OriginalTransactionId",
                table: "Refunds",
                column: "OriginalTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_RefundedToId",
                table: "Refunds",
                column: "RefundedToId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Refunds");
        }
    }
}
