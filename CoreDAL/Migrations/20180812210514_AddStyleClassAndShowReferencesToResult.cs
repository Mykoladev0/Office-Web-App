using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class AddStyleClassAndShowReferencesToResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FinalizedDate",
                table: "Shows",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ArmbandNumber",
                table: "ShowResults",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClassTemplateClassId",
                table: "ShowResults",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DogId",
                table: "ShowResults",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StyleRefId",
                table: "ShowResults",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "InActiveDate",
                table: "Judges",
                nullable: true,
                oldClrType: typeof(DateTime));
            migrationBuilder.AddPrimaryKey("PK_ClassTemplates", "ClassTemplates", "ClassId");
            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "ClassTemplates",
                nullable: true,
                maxLength: 20,
                defaultValue: "");
            //migrationBuilder.CreateTable(
            //    name: "ClassTemplates",
            //    columns: table => new
            //    {
            //        ClassId = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        StyleId = table.Column<int>(nullable: false),
            //        Name = table.Column<string>(maxLength: 50, nullable: true),
            //        Points = table.Column<int>(nullable: false),
            //        ChampWin = table.Column<bool>(nullable: false),
            //        ChampPoints = table.Column<int>(nullable: false),
            //        SortOrder = table.Column<int>(nullable: false),
            //        Gender = table.Column<string>(nullable: true, defaultValue: "")
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ClassTemplates", x => x.ClassId)
            //            .Annotation("SqlServer:Clustered", false);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Styles",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        StyleName = table.Column<string>(maxLength: 20, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Styles", x => x.Id)
            //            .Annotation("SqlServer:Clustered", false);
            //    });
            migrationBuilder.AddPrimaryKey("PK_Styles", "Styles", "Id");
            migrationBuilder.CreateIndex(
                name: "IX_ShowResults_ClassTemplateClassId",
                table: "ShowResults",
                column: "ClassTemplateClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ShowResults_ShowId",
                table: "ShowResults",
                column: "ShowId");

            migrationBuilder.CreateIndex(
                name: "IX_ShowResults_StyleRefId",
                table: "ShowResults",
                column: "StyleRefId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassTemplates_ClassId",
                table: "ClassTemplates",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Styles_Id",
                table: "Styles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "ClassTemplateId",
                table: "ShowResults",
                column: "ClassTemplateClassId",
                principalTable: "ClassTemplates",
                principalColumn: "ClassId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "ShowId",
                table: "ShowResults",
                column: "ShowId",
                principalTable: "Shows",
                principalColumn: "ShowId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "StyleId",
                table: "ShowResults",
                column: "StyleRefId",
                principalTable: "Styles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ClassTemplateId",
                table: "ShowResults");

            migrationBuilder.DropForeignKey(
                name: "ShowId",
                table: "ShowResults");

            migrationBuilder.DropForeignKey(
                name: "StyleId",
                table: "ShowResults");

            //migrationBuilder.DropTable(
            //    name: "ClassTemplates");

            //migrationBuilder.DropTable(
            //    name: "Styles");
            migrationBuilder.DropIndex(name: "IX_ClassTemplates_ClassId", table: "ClassTemplates");
            migrationBuilder.DropIndex(name: "IX_Styles_Id", table: "Styles");

            //migrationBuilder.CreateIndex(
            //    name: "id",
            //    table: "Styles",
            //    column: "Id");


            migrationBuilder.DropPrimaryKey("PK_ClassTemplates", "ClassTemplates");
            migrationBuilder.DropPrimaryKey("PK_Styles", "Styles");


            migrationBuilder.DropIndex(
                name: "IX_ShowResults_ClassTemplateClassId",
                table: "ShowResults");

            migrationBuilder.DropIndex(
                name: "IX_ShowResults_ShowId",
                table: "ShowResults");

            migrationBuilder.DropIndex(
                name: "IX_ShowResults_StyleRefId",
                table: "ShowResults");

            migrationBuilder.DropColumn(
                name: "FinalizedDate",
                table: "Shows");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "ClassTemplates");

            migrationBuilder.DropColumn(
                name: "ArmbandNumber",
                table: "ShowResults");

            migrationBuilder.DropColumn(
                name: "ClassTemplateClassId",
                table: "ShowResults");

            migrationBuilder.DropColumn(
                name: "DogId",
                table: "ShowResults");

            migrationBuilder.DropColumn(
                name: "StyleRefId",
                table: "ShowResults");

            //migrationBuilder.AlterColumn<DateTime>(
            //    name: "InActiveDate",
            //    table: "Judges",
            //    nullable: false,
            //    oldClrType: typeof(DateTime),
            //    oldNullable: true);
        }
    }
}
