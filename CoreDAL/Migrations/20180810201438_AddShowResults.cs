using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class AddShowResults : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "ShowResults",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        ShowId = table.Column<int>(nullable: false),
            //        Breed = table.Column<string>(maxLength: 30, nullable: false),
            //        Style = table.Column<string>(maxLength: 20, nullable: true),
            //        Class = table.Column<string>(maxLength: 50, nullable: true),
            //        Winning_ABKC = table.Column<string>(maxLength: 12, nullable: true),
            //        Points = table.Column<int>(nullable: false),
            //        ChampWin = table.Column<bool>(nullable: false),
            //        ChampPoints = table.Column<int>(nullable: false),
            //        NoComp = table.Column<bool>(nullable: false),
            //        Comments = table.Column<string>(maxLength: 250, nullable: true),
            //        CreateDate = table.Column<DateTime>(nullable: false),
            //        ModifyDate = table.Column<DateTime>(nullable: false),
            //        ModifiedBy = table.Column<string>(maxLength: 50, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ShowResults", x => x.Id)
            //            .Annotation("SqlServer:Clustered", false);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "id",
            //    table: "ShowResults",
            //    column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "ShowResults");
        }
    }
}
