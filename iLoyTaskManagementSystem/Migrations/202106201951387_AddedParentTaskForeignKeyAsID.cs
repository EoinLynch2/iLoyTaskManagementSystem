namespace iLoyTaskManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedParentTaskForeignKeyAsID : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.TmsTasks");
            AddColumn("dbo.TmsTasks", "ParentTmsTaskId", c => c.Int());
            AlterColumn("dbo.TmsTasks", "TmsTaskId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.TmsTasks", "TmsTaskId");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.TmsTasks");
            AlterColumn("dbo.TmsTasks", "TmsTaskId", c => c.Int(nullable: false));
            DropColumn("dbo.TmsTasks", "ParentTmsTaskId");
            AddPrimaryKey("dbo.TmsTasks", "TmsTaskId");
        }
    }
}
