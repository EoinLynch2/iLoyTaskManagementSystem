namespace iLoyTaskManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedModelForState : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.States",
                c => new
                    {
                        StateId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.StateId)
                .Index(t => t.Name, unique: true);
            
            AddColumn("dbo.TmsTasks", "State_StateId", c => c.Int(nullable: false));
            CreateIndex("dbo.TmsTasks", "State_StateId");
            AddForeignKey("dbo.TmsTasks", "State_StateId", "dbo.States", "StateId", cascadeDelete: true);
            DropColumn("dbo.TmsTasks", "State");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TmsTasks", "State", c => c.String());
            DropForeignKey("dbo.TmsTasks", "State_StateId", "dbo.States");
            DropIndex("dbo.States", new[] { "Name" });
            DropIndex("dbo.TmsTasks", new[] { "State_StateId" });
            DropColumn("dbo.TmsTasks", "State_StateId");
            DropTable("dbo.States");
        }
    }
}
