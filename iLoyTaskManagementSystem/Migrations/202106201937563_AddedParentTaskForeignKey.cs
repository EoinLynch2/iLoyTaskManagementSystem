namespace iLoyTaskManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedParentTaskForeignKey : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.TmsTasks");
            AlterColumn("dbo.TmsTasks", "TmsTaskId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.TmsTasks", "TmsTaskId");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.TmsTasks");
            AlterColumn("dbo.TmsTasks", "TmsTaskId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.TmsTasks", "TmsTaskId");
        }
    }
}
