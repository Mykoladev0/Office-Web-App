using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class addRepModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ABKCRepresentatives",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    UserRecordId = table.Column<int>(nullable: true),
                    PedigreeRegistrationFee = table.Column<double>(nullable: false),
                    LitterRegistrationFee = table.Column<double>(nullable: false),
                    PuppyRegistrationFee = table.Column<double>(nullable: false),
                    BullyIdRequestFee = table.Column<double>(nullable: false),
                    JrHandlerRegistrationFee = table.Column<double>(nullable: false),
                    TransferFee = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ABKCRepresentatives", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_ABKCRepresentatives_ABKCUsers_UserRecordId",
                        column: x => x.UserRecordId,
                        principalTable: "ABKCUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ABKCRepresentatives_Id",
                table: "ABKCRepresentatives",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ABKCRepresentatives_UserRecordId",
                table: "ABKCRepresentatives",
                column: "UserRecordId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ABKCRepresentatives");
        }
    }
}
