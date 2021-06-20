namespace iLoyTaskManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedRequiredStateField : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TmsTasks", "State_StateId", "dbo.States");
            DropIndex("dbo.TmsTasks", new[] { "State_StateId" });
            AlterColumn("dbo.TmsTasks", "State_StateId", c => c.Int());
            CreateIndex("dbo.TmsTasks", "State_StateId");
            AddForeignKey("dbo.TmsTasks", "State_StateId", "dbo.States", "StateId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TmsTasks", "State_StateId", "dbo.States");
            DropIndex("dbo.TmsTasks", new[] { "State_StateId" });
            AlterColumn("dbo.TmsTasks", "State_StateId", c => c.Int(nullable: false));
            CreateIndex("dbo.TmsTasks", "State_StateId");
            AddForeignKey("dbo.TmsTasks", "State_StateId", "dbo.States", "StateId", cascadeDelete: true);
        }
    }
}
