using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class DogIndices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Dogs_Id",
                table: "Dogs",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dogs_Id_Bully_Id_ABKC_No_DogName",
                table: "Dogs",
                columns: new[] { "Id", "Bully_Id", "ABKC_No", "DogName" })
                .Annotation("SqlServer:Clustered", false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Dogs_Id",
                table: "Dogs");

            migrationBuilder.DropIndex(
                name: "IX_Dogs_Id_Bully_Id_ABKC_No_DogName",
                table: "Dogs");
        }
    }
}
