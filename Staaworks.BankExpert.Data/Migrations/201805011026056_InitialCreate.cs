namespace Staaworks.BankExpert.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Fingerprints",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SnapshotId = c.Int(nullable: false),
                        Fmd = c.String(maxLength: 1023),
                        Fid1 = c.String(maxLength: 1023),
                        Fid2 = c.String(maxLength: 1023),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Snapshots", t => t.SnapshotId, cascadeDelete: true)
                .Index(t => t.SnapshotId);
            
            CreateTable(
                "dbo.Snapshots",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Details = c.String(maxLength: 500),
                        OwnerId = c.Int(nullable: false),
                        CreatedById = c.Int(nullable: false),
                        LastModifiedById = c.Int(),
                        DateCreated = c.DateTime(nullable: false),
                        LastModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.OwnerId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.CreatedById)
                .ForeignKey("dbo.Users", t => t.LastModifiedById)
                .Index(t => t.OwnerId)
                .Index(t => t.CreatedById)
                .Index(t => t.LastModifiedById);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        Email = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Email, unique: true);
            
            CreateTable(
                "dbo.Profiles",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Phone = c.String(maxLength: 50),
                        Address = c.String(maxLength: 255),
                        ZipOrPostalAddress = c.String(maxLength: 50),
                        ImageLocation = c.String(maxLength: 1023),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.BasicAuthData",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        PasswordHash = c.String(),
                        HashSalt = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.Id)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BasicAuthData", "Id", "dbo.Users");
            DropForeignKey("dbo.Profiles", "Id", "dbo.Users");
            DropForeignKey("dbo.Fingerprints", "SnapshotId", "dbo.Snapshots");
            DropForeignKey("dbo.Snapshots", "LastModifiedById", "dbo.Users");
            DropForeignKey("dbo.Snapshots", "CreatedById", "dbo.Users");
            DropForeignKey("dbo.Snapshots", "OwnerId", "dbo.Users");
            DropIndex("dbo.BasicAuthData", new[] { "Id" });
            DropIndex("dbo.Profiles", new[] { "Id" });
            DropIndex("dbo.Users", new[] { "Email" });
            DropIndex("dbo.Snapshots", new[] { "LastModifiedById" });
            DropIndex("dbo.Snapshots", new[] { "CreatedById" });
            DropIndex("dbo.Snapshots", new[] { "OwnerId" });
            DropIndex("dbo.Fingerprints", new[] { "SnapshotId" });
            DropTable("dbo.BasicAuthData");
            DropTable("dbo.Profiles");
            DropTable("dbo.Users");
            DropTable("dbo.Snapshots");
            DropTable("dbo.Fingerprints");
        }
    }
}
