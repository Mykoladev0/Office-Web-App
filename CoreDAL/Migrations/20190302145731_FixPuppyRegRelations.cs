using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class FixPuppyRegRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PuppyRegistrations_ABKC_Dogs_DogInfoId",
                table: "PuppyRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_PuppyRegistrations_Owners_NewCoOwnerOwnerId",
                table: "PuppyRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_PuppyRegistrations_Owners_NewOwnerOwnerId",
                table: "PuppyRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_PuppyRegistrations_DogInfoId",
                table: "PuppyRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_PuppyRegistrations_NewCoOwnerOwnerId",
                table: "PuppyRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_PuppyRegistrations_NewOwnerOwnerId",
                table: "PuppyRegistrations");

            migrationBuilder.DropColumn(
                name: "DogInfoId",
                table: "PuppyRegistrations");

            migrationBuilder.DropColumn(
                name: "NewCoOwnerOwnerId",
                table: "PuppyRegistrations");

            migrationBuilder.DropColumn(
                name: "NewOwnerOwnerId",
                table: "PuppyRegistrations");

            migrationBuilder.AddColumn<int>(
                name: "DogId",
                table: "PuppyRegistrations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NewCoOwnerId",
                table: "PuppyRegistrations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NewOwnerId",
                table: "PuppyRegistrations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PuppyRegistrations_DogId",
                table: "PuppyRegistrations",
                column: "DogId");

            migrationBuilder.CreateIndex(
                name: "IX_PuppyRegistrations_NewCoOwnerId",
                table: "PuppyRegistrations",
                column: "NewCoOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PuppyRegistrations_NewOwnerId",
                table: "PuppyRegistrations",
                column: "NewOwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_PuppyRegistrations_ABKC_Dogs_DogId",
                table: "PuppyRegistrations",
                column: "DogId",
                principalTable: "ABKC_Dogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PuppyRegistrations_Owners_NewCoOwnerId",
                table: "PuppyRegistrations",
                column: "NewCoOwnerId",
                principalTable: "Owners",
                principalColumn: "Owner_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PuppyRegistrations_Owners_NewOwnerId",
                table: "PuppyRegistrations",
                column: "NewOwnerId",
                principalTable: "Owners",
                principalColumn: "Owner_Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PuppyRegistrations_ABKC_Dogs_DogId",
                table: "PuppyRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_PuppyRegistrations_Owners_NewCoOwnerId",
                table: "PuppyRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_PuppyRegistrations_Owners_NewOwnerId",
                table: "PuppyRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_PuppyRegistrations_DogId",
                table: "PuppyRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_PuppyRegistrations_NewCoOwnerId",
                table: "PuppyRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_PuppyRegistrations_NewOwnerId",
                table: "PuppyRegistrations");

            migrationBuilder.DropColumn(
                name: "DogId",
                table: "PuppyRegistrations");

            migrationBuilder.DropColumn(
                name: "NewCoOwnerId",
                table: "PuppyRegistrations");

            migrationBuilder.DropColumn(
                name: "NewOwnerId",
                table: "PuppyRegistrations");

            migrationBuilder.AddColumn<int>(
                name: "DogInfoId",
                table: "PuppyRegistrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NewCoOwnerOwnerId",
                table: "PuppyRegistrations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NewOwnerOwnerId",
                table: "PuppyRegistrations",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PuppyRegistrations_DogInfoId",
                table: "PuppyRegistrations",
                column: "DogInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_PuppyRegistrations_NewCoOwnerOwnerId",
                table: "PuppyRegistrations",
                column: "NewCoOwnerOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PuppyRegistrations_NewOwnerOwnerId",
                table: "PuppyRegistrations",
                column: "NewOwnerOwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_PuppyRegistrations_ABKC_Dogs_DogInfoId",
                table: "PuppyRegistrations",
                column: "DogInfoId",
                principalTable: "ABKC_Dogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PuppyRegistrations_Owners_NewCoOwnerOwnerId",
                table: "PuppyRegistrations",
                column: "NewCoOwnerOwnerId",
                principalTable: "Owners",
                principalColumn: "Owner_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PuppyRegistrations_Owners_NewOwnerOwnerId",
                table: "PuppyRegistrations",
                column: "NewOwnerOwnerId",
                principalTable: "Owners",
                principalColumn: "Owner_Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
