using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace GoGoNihon_MVC.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        
        
        public List<UserLanguage> userLanguage { get; set; }


    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        //public DbSet<GoGoUser> GoGoUser { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<Content> Content { get; set; }
        public DbSet<ContentBody> ContentBody { get; set; }
        public DbSet<Language> Language { get; set; }
        public DbSet<UserLanguage> UserLanguages { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<SchoolStation> Stations { get; set; }
        public DbSet<SchoolCourse> schoolCourses { get; set; }
        public DbSet<CourseLength> courseLengths { get; set; }
        public DbSet<TermBreakdownStep> termSteps { get; set; }
        public DbSet<CourseDemography> demographies { get; set; }
        public DbSet<ImageGallery> gallerys { get; set; }
        public DbSet<GalleryImage> galleryImages { get; set; }
        public DbSet<ShortCourse> shortCourses { get; set; }
        public DbSet<PlanFeature> planFeatures { get; set; }
        public DbSet<Faq> faqs { get; set; }
        public DbSet<FaqQuestion> faqQuestions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Page>().HasMany<Content>(c => c.content).WithOptional().WillCascadeOnDelete();
            modelBuilder.Entity<Content>().HasMany<ContentBody>(c => c.contentCollection).WithOptional().WillCascadeOnDelete();
            modelBuilder.Entity<ApplicationUser>().HasMany<UserLanguage>(a => a.userLanguage);
            modelBuilder.Entity<Location>().HasOptional<ImageGallery>(l => l.locationGallery);
            modelBuilder.Entity<School>().HasOptional<Location>(s => s.schoolLocation);
            modelBuilder.Entity<School>().HasMany<SchoolStation>(s => s.schoolStations).WithOptional().WillCascadeOnDelete();
            modelBuilder.Entity<School>().HasMany<Content>(s => s.content).WithOptional().WillCascadeOnDelete();
            modelBuilder.Entity<ImageGallery>().HasMany<GalleryImage>(s => s.galleryImages).WithOptional().WillCascadeOnDelete();
            modelBuilder.Entity<GalleryImage>().HasMany<Content>(s => s.content).WithOptional().WillCascadeOnDelete();
            modelBuilder.Entity<ShortCourse>().HasMany<Content>(s => s.content).WithOptional().WillCascadeOnDelete();
            modelBuilder.Entity<FaqQuestion>().HasMany<Content>(s => s.content).WithOptional().WillCascadeOnDelete();
            modelBuilder.Entity<Faq>().HasMany<FaqQuestion>(s => s.questions).WithOptional().WillCascadeOnDelete();
            modelBuilder.Entity<Location>().HasMany<Content>(c => c.contentCollection).WithOptional().WillCascadeOnDelete();

        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}