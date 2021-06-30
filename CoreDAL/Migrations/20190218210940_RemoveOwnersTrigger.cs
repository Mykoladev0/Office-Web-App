using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class RemoveOwnersTrigger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string dropTriggerString = "DROP TRIGGER [dbo].[tSetOwnerId]";
            migrationBuilder.Sql(dropTriggerString);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string addTriggerString = "create trigger [dbo].[tSetOwnerId] on [dbo].[Owners] AFTER Insert as set nocount on declare @LastOwnerId  int Set @LastOwnerId = (SELECT LastOwnerId FROM Defaults) UPDATE Owners  SET Owner_id = (@LastOwnerId + 1) FROM inserted i JOIN Owners o ON i.ID = o.ID UPDATE Defaults SET LastOwnerId = (@LastOwnerId + 1)";
            migrationBuilder.Sql(addTriggerString);
            migrationBuilder.Sql("ALTER TABLE[dbo].[Owners] ENABLE TRIGGER[tSetOwnerId]");
        }
    }
}
