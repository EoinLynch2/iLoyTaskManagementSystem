namespace iLoyTaskManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class recreatetable : DbMigration
    {
        public override void Up()
        {
            
            
            
            CreateTable(
                "dbo.TmsTasks",
                c => new
                    {
                        TmsTaskId = c.Int(nullable: false, identity: true),
                        TaskName = c.String(maxLength: 200),
                        Description = c.String(),
                        StartDate = c.DateTimeOffset(nullable: false, precision: 7),
                        FinishDate = c.DateTimeOffset(nullable: false, precision: 7),
                        State = c.String(),
                        ParentTmsTaskId = c.Int(),
                    })
                .PrimaryKey(t => t.TmsTaskId)
                .Index(t => t.TaskName, unique: true);
            
            
            
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.TmsTasks", new[] { "TaskName" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.TmsTasks");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
        }
    }
}
