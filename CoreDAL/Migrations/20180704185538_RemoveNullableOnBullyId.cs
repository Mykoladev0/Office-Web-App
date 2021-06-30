using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class RemoveNullableOnBullyId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //need to remove this because we are altering the Bully_Id table
            migrationBuilder.DropIndex(name: "IX_Dogs", table: "Dogs");
            migrationBuilder.DropIndex(name: "nci_wi_Dogs_04C4DBCA2A103E7B5012D171327316CB", table: "Dogs");
            migrationBuilder.DropIndex(name: "nci_wi_Dogs_09156819B470C0F84864004C1B91B0BD", table: "Dogs");
            migrationBuilder.DropIndex(name: "nci_wi_Dogs_1010D5EF05F29DEEB1DA1F5947BFD478", table: "Dogs");
            migrationBuilder.DropIndex(name: "nci_wi_Dogs_A3E73A88B504C25476BC6C69921244C4", table: "Dogs");
            migrationBuilder.AlterColumn<int>(
                name: "Bully_Id",
                table: "Dogs",
                nullable: false,
                defaultValueSql: "((0))",
                oldClrType: typeof(int),
                oldNullable: true,
                oldDefaultValueSql: "((0))");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Bully_Id",
                table: "Dogs",
                nullable: true,
                defaultValueSql: "((0))",
                oldClrType: typeof(int),
                oldDefaultValueSql: "((0))");

            //add old index (don't know if this is right!)
            migrationBuilder.CreateIndex(
                name: "IX_Dogs",
                table: "Dogs",
                columns: new[] { "Id", "Bully_Id", "ABKC_No", "DogName" })
                .Annotation("SqlServer:Clustered", false);
        }
    }
}
