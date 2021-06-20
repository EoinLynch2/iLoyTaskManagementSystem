namespace iLoyTaskManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClarifiedNamesOfTaskAndState : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.TmsTasks", new[] { "Name" });
            DropIndex("dbo.States", new[] { "Name" });
            AddColumn("dbo.TmsTasks", "TaskName", c => c.String(maxLength: 200));
            AddColumn("dbo.States", "StateName", c => c.String(maxLength: 200));
            CreateIndex("dbo.States", "StateName", unique: true);
            CreateIndex("dbo.TmsTasks", "TaskName", unique: true);
            DropColumn("dbo.TmsTasks", "Name");
            DropColumn("dbo.States", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.States", "Name", c => c.String(maxLength: 200));
            AddColumn("dbo.TmsTasks", "Name", c => c.String(maxLength: 200));
            DropIndex("dbo.TmsTasks", new[] { "TaskName" });
            DropIndex("dbo.States", new[] { "StateName" });
            DropColumn("dbo.States", "StateName");
            DropColumn("dbo.TmsTasks", "TaskName");
            CreateIndex("dbo.States", "Name", unique: true);
            CreateIndex("dbo.TmsTasks", "Name", unique: true);
        }
    }
}
