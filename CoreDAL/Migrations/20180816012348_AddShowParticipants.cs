using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class AddShowParticipants : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShowParticipants",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ArmbandNumber = table.Column<int>(nullable: true),
                    ShowId1 = table.Column<int>(nullable: false),
                    DogId1 = table.Column<int>(nullable: false),
                    DateRegistered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowParticipants", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_ShowParticipants_Dogs_DogId1",
                        column: x => x.DogId1,
                        principalTable: "Dogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShowParticipants_Shows_ShowId1",
                        column: x => x.ShowId1,
                        principalTable: "Shows",
                        principalColumn: "ShowId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShowParticipants_DogId1",
                table: "ShowParticipants",
                column: "DogId1");

            migrationBuilder.CreateIndex(
                name: "IX_ShowParticipants_ShowId1",
                table: "ShowParticipants",
                column: "ShowId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShowParticipants");
        }
    }
}
