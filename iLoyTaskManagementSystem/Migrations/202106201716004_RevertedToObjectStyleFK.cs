namespace iLoyTaskManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RevertedToObjectStyleFK : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TmsTasks", "StateID", "dbo.States");
            RenameColumn(table: "dbo.TmsTasks", name: "StateID", newName: "State_StateId");
            RenameIndex(table: "dbo.TmsTasks", name: "IX_StateID", newName: "IX_State_StateId");
            DropPrimaryKey("dbo.States");
            AddColumn("dbo.States", "StateId", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.States", "StateName", c => c.String(maxLength: 200));
            AddPrimaryKey("dbo.States", "StateId");
            CreateIndex("dbo.States", "StateName", unique: true);
            AddForeignKey("dbo.TmsTasks", "State_StateId", "dbo.States", "StateId", cascadeDelete: true);
            DropColumn("dbo.States", "Id");
            DropColumn("dbo.States", "StatusName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.States", "StatusName", c => c.String());
            AddColumn("dbo.States", "Id", c => c.Int(nullable: false));
            DropForeignKey("dbo.TmsTasks", "State_StateId", "dbo.States");
            DropIndex("dbo.States", new[] { "StateName" });
            DropPrimaryKey("dbo.States");
            DropColumn("dbo.States", "StateName");
            DropColumn("dbo.States", "StateId");
            AddPrimaryKey("dbo.States", "Id");
            RenameIndex(table: "dbo.TmsTasks", name: "IX_State_StateId", newName: "IX_StateID");
            RenameColumn(table: "dbo.TmsTasks", name: "State_StateId", newName: "StateID");
            AddForeignKey("dbo.TmsTasks", "StateID", "dbo.States", "Id", cascadeDelete: true);
        }
    }
}
