namespace iLoyTaskManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MadeTaskNameUnique : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TmsTasks", "Name", c => c.String(maxLength: 200));
            CreateIndex("dbo.TmsTasks", "Name", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.TmsTasks", new[] { "Name" });
            AlterColumn("dbo.TmsTasks", "Name", c => c.String());
        }
    }
}
