namespace GoGoNihon_MVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Contents",
                c => new
                    {
                        contentID = c.Int(nullable: false, identity: true),
                        pageID = c.Int(nullable: false),
                        name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.contentID)
                .ForeignKey("dbo.Pages", t => t.pageID, cascadeDelete: true)
                .Index(t => t.pageID);
            
            CreateTable(
                "dbo.ContentBodies",
                c => new
                    {
                        contentBodyID = c.Int(nullable: false, identity: true),
                        contentID = c.Int(nullable: false),
                        code = c.String(nullable: false, maxLength: 128),
                        body = c.String(),
                        lastModified = c.String(),
                        lastModifiedByID = c.String(),
                    })
                .PrimaryKey(t => t.contentBodyID)
                .ForeignKey("dbo.Languages", t => t.code, cascadeDelete: true)
                .ForeignKey("dbo.Contents", t => t.contentID, cascadeDelete: true)
                .Index(t => t.contentID)
                .Index(t => t.code);
            
            CreateTable(
                "dbo.Languages",
                c => new
                    {
                        code = c.String(nullable: false, maxLength: 128),
                        name = c.String(),
                        flag = c.String(),
                        Content_contentID = c.Int(),
                    })
                .PrimaryKey(t => t.code)
                .ForeignKey("dbo.Contents", t => t.Content_contentID)
                .Index(t => t.Content_contentID);
            
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        locationID = c.Int(nullable: false, identity: true),
                        image = c.String(),
                        name = c.String(),
                        description = c.String(),
                        locationGallery_id = c.Int(),
                    })
                .PrimaryKey(t => t.locationID)
                .ForeignKey("dbo.ImageGalleries", t => t.locationGallery_id)
                .Index(t => t.locationGallery_id);
            
            CreateTable(
                "dbo.ImageGalleries",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.GalleryImages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        image = c.String(nullable: false),
                        galleryID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ImageGalleries", t => t.galleryID, cascadeDelete: true)
                .Index(t => t.galleryID);
            
            CreateTable(
                "dbo.Pages",
                c => new
                    {
                        pageID = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false),
                        description = c.String(),
                        path = c.String(nullable: false),
                        controller = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.pageID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Schools",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        type = c.Int(nullable: false),
                        name = c.String(),
                        previewVideo = c.String(),
                        video = c.String(),
                        videoCover = c.String(),
                        introduction = c.String(),
                        introductionImage = c.String(),
                        intensity = c.Int(nullable: false),
                        intensityImage = c.String(),
                        featuresImage = c.String(),
                        features_greatForWestern = c.Boolean(nullable: false),
                        features_acceptsBeginners = c.Boolean(nullable: false),
                        features_wifiInCommonAreas = c.Boolean(nullable: false),
                        features_studentLounge = c.Boolean(nullable: false),
                        features_studentLounge24hour = c.Boolean(nullable: false),
                        features_englishStaff = c.Boolean(nullable: false),
                        features_fullTimeJobSupport = c.Boolean(nullable: false),
                        features_partTimeJobSupport = c.Boolean(nullable: false),
                        features_interactWithJapanese = c.Boolean(nullable: false),
                        features_slowPaced = c.Boolean(nullable: false),
                        features_smallClassSizes = c.Boolean(nullable: false),
                        features_studentDorms = c.Boolean(nullable: false),
                        features_uniqueFeature = c.Boolean(nullable: false),
                        extraSpecialFeature = c.String(),
                        locationID = c.Int(),
                        address = c.String(),
                        googleMap = c.String(),
                        coursesImage = c.String(),
                        termStartNotes = c.String(),
                        extraSpecialFeatureGallery_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.ImageGalleries", t => t.extraSpecialFeatureGallery_id)
                .ForeignKey("dbo.Locations", t => t.locationID)
                .Index(t => t.locationID)
                .Index(t => t.extraSpecialFeatureGallery_id);
            
            CreateTable(
                "dbo.SchoolCourses",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        introduction = c.String(),
                        image = c.String(),
                        hoursPerWeek = c.Int(nullable: false),
                        scheduleText = c.String(),
                        termBreakdownText = c.String(),
                        totalStudents = c.Int(nullable: false),
                        studentsPerClass = c.Int(nullable: false),
                        demographyImage = c.String(),
                        schoolID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Schools", t => t.schoolID, cascadeDelete: true)
                .Index(t => t.schoolID);
            
            CreateTable(
                "dbo.CourseDemographies",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false),
                        percent = c.Int(nullable: false),
                        schoolCourseID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.SchoolCourses", t => t.schoolCourseID, cascadeDelete: true)
                .Index(t => t.schoolCourseID);
            
            CreateTable(
                "dbo.TermBreakdownSteps",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        index = c.Int(nullable: false),
                        body = c.String(),
                        schoolCourseID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.SchoolCourses", t => t.schoolCourseID, cascadeDelete: true)
                .Index(t => t.schoolCourseID);
            
            CreateTable(
                "dbo.SchoolStations",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false),
                        image = c.String(),
                        line = c.String(nullable: false),
                        distance = c.String(nullable: false),
                        schoolID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Schools", t => t.schoolID, cascadeDelete: true)
                .Index(t => t.schoolID);
            
            CreateTable(
                "dbo.SchoolContents",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        schoolID = c.Int(nullable: false),
                        body = c.String(nullable: false),
                        code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Languages", t => t.code)
                .ForeignKey("dbo.Schools", t => t.schoolID, cascadeDelete: true)
                .Index(t => t.schoolID)
                .Index(t => t.code);
            
            CreateTable(
                "dbo.UserLanguages",
                c => new
                    {
                        userLanguageID = c.Int(nullable: false, identity: true),
                        code = c.String(nullable: false, maxLength: 128),
                        userID = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.userLanguageID)
                .ForeignKey("dbo.Languages", t => t.code, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.userID, cascadeDelete: true)
                .Index(t => t.code)
                .Index(t => t.userID);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserLanguages", "userID", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserLanguages", "code", "dbo.Languages");
            DropForeignKey("dbo.SchoolContents", "schoolID", "dbo.Schools");
            DropForeignKey("dbo.SchoolContents", "code", "dbo.Languages");
            DropForeignKey("dbo.SchoolStations", "schoolID", "dbo.Schools");
            DropForeignKey("dbo.Schools", "locationID", "dbo.Locations");
            DropForeignKey("dbo.TermBreakdownSteps", "schoolCourseID", "dbo.SchoolCourses");
            DropForeignKey("dbo.SchoolCourses", "schoolID", "dbo.Schools");
            DropForeignKey("dbo.CourseDemographies", "schoolCourseID", "dbo.SchoolCourses");
            DropForeignKey("dbo.Schools", "extraSpecialFeatureGallery_id", "dbo.ImageGalleries");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Contents", "pageID", "dbo.Pages");
            DropForeignKey("dbo.Locations", "locationGallery_id", "dbo.ImageGalleries");
            DropForeignKey("dbo.GalleryImages", "galleryID", "dbo.ImageGalleries");
            DropForeignKey("dbo.Languages", "Content_contentID", "dbo.Contents");
            DropForeignKey("dbo.ContentBodies", "contentID", "dbo.Contents");
            DropForeignKey("dbo.ContentBodies", "code", "dbo.Languages");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.UserLanguages", new[] { "userID" });
            DropIndex("dbo.UserLanguages", new[] { "code" });
            DropIndex("dbo.SchoolContents", new[] { "code" });
            DropIndex("dbo.SchoolContents", new[] { "schoolID" });
            DropIndex("dbo.SchoolStations", new[] { "schoolID" });
            DropIndex("dbo.TermBreakdownSteps", new[] { "schoolCourseID" });
            DropIndex("dbo.CourseDemographies", new[] { "schoolCourseID" });
            DropIndex("dbo.SchoolCourses", new[] { "schoolID" });
            DropIndex("dbo.Schools", new[] { "extraSpecialFeatureGallery_id" });
            DropIndex("dbo.Schools", new[] { "locationID" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.GalleryImages", new[] { "galleryID" });
            DropIndex("dbo.Locations", new[] { "locationGallery_id" });
            DropIndex("dbo.Languages", new[] { "Content_contentID" });
            DropIndex("dbo.ContentBodies", new[] { "code" });
            DropIndex("dbo.ContentBodies", new[] { "contentID" });
            DropIndex("dbo.Contents", new[] { "pageID" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.UserLanguages");
            DropTable("dbo.SchoolContents");
            DropTable("dbo.SchoolStations");
            DropTable("dbo.TermBreakdownSteps");
            DropTable("dbo.CourseDemographies");
            DropTable("dbo.SchoolCourses");
            DropTable("dbo.Schools");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Pages");
            DropTable("dbo.GalleryImages");
            DropTable("dbo.ImageGalleries");
            DropTable("dbo.Locations");
            DropTable("dbo.Languages");
            DropTable("dbo.ContentBodies");
            DropTable("dbo.Contents");
        }
    }
}
