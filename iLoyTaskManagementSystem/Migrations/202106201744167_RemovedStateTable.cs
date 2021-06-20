namespace iLoyTaskManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedStateTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TmsTasks", "ParentTmsTask_TmsTaskId", "dbo.TmsTasks");
            DropForeignKey("dbo.TmsTasks", "State_StateId", "dbo.States");
            DropIndex("dbo.States", new[] { "StateName" });
            DropIndex("dbo.TmsTasks", new[] { "ParentTmsTask_TmsTaskId" });
            DropIndex("dbo.TmsTasks", new[] { "State_StateId" });
            AddColumn("dbo.TmsTasks", "State", c => c.String());
            DropColumn("dbo.TmsTasks", "ParentTmsTask_TmsTaskId");
            DropColumn("dbo.TmsTasks", "State_StateId");
            DropTable("dbo.States");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.States",
                c => new
                    {
                        StateId = c.Int(nullable: false, identity: true),
                        StateName = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.StateId);
            
            AddColumn("dbo.TmsTasks", "State_StateId", c => c.Int());
            AddColumn("dbo.TmsTasks", "ParentTmsTask_TmsTaskId", c => c.Int());
            DropColumn("dbo.TmsTasks", "State");
            CreateIndex("dbo.TmsTasks", "State_StateId");
            CreateIndex("dbo.TmsTasks", "ParentTmsTask_TmsTaskId");
            CreateIndex("dbo.States", "StateName", unique: true);
            AddForeignKey("dbo.TmsTasks", "State_StateId", "dbo.States", "StateId");
            AddForeignKey("dbo.TmsTasks", "ParentTmsTask_TmsTaskId", "dbo.TmsTasks", "TmsTaskId");
        }
    }
}
