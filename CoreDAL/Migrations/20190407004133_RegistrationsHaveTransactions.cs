using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class RegistrationsHaveTransactions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssociatedTransactionId",
                table: "PuppyRegistrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssociatedTransactionId",
                table: "LitterRegistrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssociatedTransactionId",
                table: "JrHandlerRegistrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssociatedTransactionId",
                table: "BullyIdRequests",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PuppyRegistrations_AssociatedTransactionId",
                table: "PuppyRegistrations",
                column: "AssociatedTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_LitterRegistrations_AssociatedTransactionId",
                table: "LitterRegistrations",
                column: "AssociatedTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_JrHandlerRegistrations_AssociatedTransactionId",
                table: "JrHandlerRegistrations",
                column: "AssociatedTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_BullyIdRequests_AssociatedTransactionId",
                table: "BullyIdRequests",
                column: "AssociatedTransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_BullyIdRequests_Transactions_AssociatedTransactionId",
                table: "BullyIdRequests",
                column: "AssociatedTransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JrHandlerRegistrations_Transactions_AssociatedTransactionId",
                table: "JrHandlerRegistrations",
                column: "AssociatedTransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LitterRegistrations_Transactions_AssociatedTransactionId",
                table: "LitterRegistrations",
                column: "AssociatedTransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PuppyRegistrations_Transactions_AssociatedTransactionId",
                table: "PuppyRegistrations",
                column: "AssociatedTransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BullyIdRequests_Transactions_AssociatedTransactionId",
                table: "BullyIdRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_JrHandlerRegistrations_Transactions_AssociatedTransactionId",
                table: "JrHandlerRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_LitterRegistrations_Transactions_AssociatedTransactionId",
                table: "LitterRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_PuppyRegistrations_Transactions_AssociatedTransactionId",
                table: "PuppyRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_PuppyRegistrations_AssociatedTransactionId",
                table: "PuppyRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_LitterRegistrations_AssociatedTransactionId",
                table: "LitterRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_JrHandlerRegistrations_AssociatedTransactionId",
                table: "JrHandlerRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_BullyIdRequests_AssociatedTransactionId",
                table: "BullyIdRequests");

            migrationBuilder.DropColumn(
                name: "AssociatedTransactionId",
                table: "PuppyRegistrations");

            migrationBuilder.DropColumn(
                name: "AssociatedTransactionId",
                table: "LitterRegistrations");

            migrationBuilder.DropColumn(
                name: "AssociatedTransactionId",
                table: "JrHandlerRegistrations");

            migrationBuilder.DropColumn(
                name: "AssociatedTransactionId",
                table: "BullyIdRequests");
        }
    }
}
