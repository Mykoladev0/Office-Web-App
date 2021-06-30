using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class ChangeGenderFieldLengthTo10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "Dogs",
                unicode: false,
                maxLength: 10,
                nullable: true,
                defaultValueSql: "('')",
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 6,
                oldNullable: true,
                oldDefaultValueSql: "('')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "Dogs",
                unicode: false,
                maxLength: 6,
                nullable: true,
                defaultValueSql: "('')",
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 10,
                oldNullable: true,
                oldDefaultValueSql: "('')");
        }
    }
}
