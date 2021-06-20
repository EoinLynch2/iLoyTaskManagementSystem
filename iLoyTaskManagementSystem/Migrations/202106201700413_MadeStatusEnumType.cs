namespace iLoyTaskManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MadeStatusEnumType : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TmsTasks", "State_StateId", "dbo.States");
            DropIndex("dbo.States", new[] { "StateName" });
            RenameColumn(table: "dbo.TmsTasks", name: "State_StateId", newName: "StateID");
            RenameIndex(table: "dbo.TmsTasks", name: "IX_State_StateId", newName: "IX_StateID");
            DropPrimaryKey("dbo.States");
            AddColumn("dbo.States", "Id", c => c.Int(nullable: false));
            AddColumn("dbo.States", "StatusName", c => c.String());
            AddPrimaryKey("dbo.States", "Id");
            AddForeignKey("dbo.TmsTasks", "StateID", "dbo.States", "Id", cascadeDelete: true);
            DropColumn("dbo.States", "StateId");
            DropColumn("dbo.States", "StateName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.States", "StateName", c => c.String(maxLength: 200));
            AddColumn("dbo.States", "StateId", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.TmsTasks", "StateID", "dbo.States");
            DropPrimaryKey("dbo.States");
            DropColumn("dbo.States", "StatusName");
            DropColumn("dbo.States", "Id");
            AddPrimaryKey("dbo.States", "StateId");
            RenameIndex(table: "dbo.TmsTasks", name: "IX_StateID", newName: "IX_State_StateId");
            RenameColumn(table: "dbo.TmsTasks", name: "StateID", newName: "State_StateId");
            CreateIndex("dbo.States", "StateName", unique: true);
            AddForeignKey("dbo.TmsTasks", "State_StateId", "dbo.States", "StateId", cascadeDelete: true);
        }
    }
}
