using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace CoreDAL.Migrations
{
    public partial class SwitchResultsDateColumnsToDateTime2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(name: "ModifyDate", table: "ShowResults", nullable: true,
                type:"datetime2",
                oldNullable:true,
                oldClrType: typeof(DateTime));
            migrationBuilder.AlterColumn<DateTime>(name: "CreateDate", table: "ShowResults", nullable: true,
                oldNullable: true,
                type: "datetime2",oldClrType: typeof(DateTime));

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
