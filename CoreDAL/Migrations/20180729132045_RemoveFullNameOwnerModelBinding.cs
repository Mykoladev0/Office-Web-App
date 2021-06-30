using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class RemoveFullNameOwnerModelBinding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "FullName",
            //    table: "Owners");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<string>(
            //    name: "FullName",
            //    table: "Owners",
            //    unicode: false,
            //    maxLength: 50,
            //    nullable: true);
        }
    }
}
