namespace iLoyTaskManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedRequiredFromState : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.States", new[] { "StateName" });
            AlterColumn("dbo.States", "StateName", c => c.String(maxLength: 200));
            CreateIndex("dbo.States", "StateName", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.States", new[] { "StateName" });
            AlterColumn("dbo.States", "StateName", c => c.String(nullable: false, maxLength: 200));
            CreateIndex("dbo.States", "StateName", unique: true);
        }
    }
}
