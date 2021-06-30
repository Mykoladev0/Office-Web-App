using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class RemoveStateCharacterLimit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "State",
                table: "Owners",
                unicode: false,
                nullable: true,
                defaultValueSql: "('')",
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 3,
                oldNullable: true,
                oldDefaultValueSql: "('')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "State",
                table: "Owners",
                unicode: false,
                maxLength: 3,
                nullable: true,
                defaultValueSql: "('')",
                oldClrType: typeof(string),
                oldUnicode: false,
                oldNullable: true,
                oldDefaultValueSql: "('')");
        }
    }
}
