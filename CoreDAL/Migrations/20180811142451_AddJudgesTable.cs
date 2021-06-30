using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class AddJudgesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AlterColumn<bool>(
            //    name: "NoComp",
            //    table: "ShowResults",
            //    nullable: true,
            //    oldClrType: typeof(bool));

            //migrationBuilder.CreateTable(
            //    name: "Judges",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        FirstName = table.Column<string>(maxLength: 50, nullable: true),
            //        LastName = table.Column<string>(maxLength: 50, nullable: true),
            //        IsActive = table.Column<bool>(nullable: false),
            //        InActiveDate = table.Column<DateTime>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Judges", x => x.Id)
            //            .Annotation("SqlServer:Clustered", false);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "id",
            //    table: "Judges",
            //    column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "Judges");

            //migrationBuilder.AlterColumn<bool>(
            //    name: "NoComp",
            //    table: "ShowResults",
            //    nullable: false,
            //    oldClrType: typeof(bool),
            //    oldNullable: true);
        }
    }
}
