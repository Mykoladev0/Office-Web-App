using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class transferLinToPuppyAndSupportingDocs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BillOfSaleBackId",
                table: "PuppyRegistrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BillOfSaleFrontId",
                table: "PuppyRegistrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TransferCreatedFromRegistrationId",
                table: "PuppyRegistrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FrontPhotoId",
                table: "BullyIdRequests",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PuppyRegistrations_BillOfSaleBackId",
                table: "PuppyRegistrations",
                column: "BillOfSaleBackId");

            migrationBuilder.CreateIndex(
                name: "IX_PuppyRegistrations_BillOfSaleFrontId",
                table: "PuppyRegistrations",
                column: "BillOfSaleFrontId");

            migrationBuilder.CreateIndex(
                name: "IX_PuppyRegistrations_TransferCreatedFromRegistrationId",
                table: "PuppyRegistrations",
                column: "TransferCreatedFromRegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_BullyIdRequests_FrontPhotoId",
                table: "BullyIdRequests",
                column: "FrontPhotoId");

            migrationBuilder.AddForeignKey(
                name: "FK_BullyIdRequests_Attachments_FrontPhotoId",
                table: "BullyIdRequests",
                column: "FrontPhotoId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PuppyRegistrations_Attachments_BillOfSaleBackId",
                table: "PuppyRegistrations",
                column: "BillOfSaleBackId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PuppyRegistrations_Attachments_BillOfSaleFrontId",
                table: "PuppyRegistrations",
                column: "BillOfSaleFrontId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PuppyRegistrations_Transfers_TransferCreatedFromRegistrationId",
                table: "PuppyRegistrations",
                column: "TransferCreatedFromRegistrationId",
                principalTable: "Transfers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BullyIdRequests_Attachments_FrontPhotoId",
                table: "BullyIdRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_PuppyRegistrations_Attachments_BillOfSaleBackId",
                table: "PuppyRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_PuppyRegistrations_Attachments_BillOfSaleFrontId",
                table: "PuppyRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_PuppyRegistrations_Transfers_TransferCreatedFromRegistrationId",
                table: "PuppyRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_PuppyRegistrations_BillOfSaleBackId",
                table: "PuppyRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_PuppyRegistrations_BillOfSaleFrontId",
                table: "PuppyRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_PuppyRegistrations_TransferCreatedFromRegistrationId",
                table: "PuppyRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_BullyIdRequests_FrontPhotoId",
                table: "BullyIdRequests");

            migrationBuilder.DropColumn(
                name: "BillOfSaleBackId",
                table: "PuppyRegistrations");

            migrationBuilder.DropColumn(
                name: "BillOfSaleFrontId",
                table: "PuppyRegistrations");

            migrationBuilder.DropColumn(
                name: "TransferCreatedFromRegistrationId",
                table: "PuppyRegistrations");

            migrationBuilder.DropColumn(
                name: "FrontPhotoId",
                table: "BullyIdRequests");
        }
    }
}
