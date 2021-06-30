using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class specialRequestToJrHandler : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsInternationalRegistration",
                table: "JrHandlerRegistrations",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OvernightRequested",
                table: "JrHandlerRegistrations",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RushRequested",
                table: "JrHandlerRegistrations",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInternationalRegistration",
                table: "JrHandlerRegistrations");

            migrationBuilder.DropColumn(
                name: "OvernightRequested",
                table: "JrHandlerRegistrations");

            migrationBuilder.DropColumn(
                name: "RushRequested",
                table: "JrHandlerRegistrations");
        }
    }
}
