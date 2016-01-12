using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace GoGoNihon_MVC.Models
{

    public enum SchoolTypes
    {
        language,
        vocational

    }

    public class School
    {
        [Key]
        [Required]
        public int id { get; set; }

        public int type { get; set; }




        public List<Content> content { get; set; }

        public string url { get; set; }

        public string previewVideo { get; set; }
        public string video { get; set; }
        public string videoCover { get; set; }

        public string introductionImage { get; set; }

        public int intensity { get; set; }
        public string intensityImage { get; set; }

        
        public string featuresImage { get; set; }

        //[ForeignKey("id")]
        public SchoolFeatures features { get; set; }

        public string extraSpecialFeature { get; set; }
        public ImageGallery extraSpecialFeatureGallery { get; set; }

        
        public int? locationID { get; set; }

        [ForeignKey("locationID")]
        public virtual Location schoolLocation { get; set; }
        
        public string address { get; set; }
        public string googleMap { get; set; }

        public ICollection<SchoolStation> schoolStations { get; set; }


        public string coursesImage { get; set; }
        public ICollection<SchoolCourse> schoolCourses { get; set; }


        //public string termStartNotes {
        //    get { return setVariable("termStartNotes"); }
        //    set { }
        //}


        public string languageCode = "en";

        public School()
        {
            //empty constructor
        }

        public School(string languageCode)
        {
            content = new List<Content>();
            this.languageCode = languageCode;
        }

        public School(int id, string languageCode)
        {
            this.id = id;
            content = new List<Content>();
            using (ApplicationDbContext DB = new ApplicationDbContext())
            {
                
                this.languageCode = languageCode;
                loadContent(DB);
            }
            
        }

        public void loadContent(ApplicationDbContext DB, bool widthAdmin=false)
        {
            content = DB.Content.Where(c => c.schoolID == this.id).ToList();

            foreach (Content c in content)
            {
                c.loadContent(languageCode);
            }

            schoolLocation = DB.Locations.Where(l => l.id == locationID).FirstOrDefault();
            schoolStations = DB.Stations.Where(l => l.schoolID == id).ToList();
            

            if (schoolLocation != null)
            {
                schoolLocation.loadContent(languageCode);
            }

            if (widthAdmin)
            {
                doAdminControls(languageCode);
            }
            
        }


        public bool addContent(ApplicationDbContext DB, string name, string body)
        {
           
            if (DB.Content.Where(c => c.schoolID == id && c.name == name).Any())
            {
                Content c = DB.Content.Where(c2 => c2.schoolID == id && c2.name == name).FirstOrDefault();

                if (DB.ContentBody.Where(cb2 => cb2.contentID == c.contentID && cb2.code == languageCode).Any())
                {
                    ContentBody cb3 = DB.ContentBody.Where(cb2 => cb2.contentID == c.contentID && cb2.code == languageCode).FirstOrDefault();
                    cb3.body = body;
                    cb3.lastModified = DateTime.Now;
                    cb3.lastModifiedByID = HttpContext.Current.User.Identity.GetUserId();
                    DB.SaveChanges();
                }
                else
                {

                    ContentBody cb3 = new ContentBody();
                    cb3.contentID = c.contentID;
                    cb3.code = languageCode;
                    cb3.lastModified = DateTime.Now;
                    cb3.lastModifiedByID = HttpContext.Current.User.Identity.GetUserId();
                    cb3.body = body;
                    DB.ContentBody.Add(cb3);
                    DB.SaveChanges();
                }

            }
            else
            {
                Content content = new Content();
                content.schoolID = id;
                content.name = name;
                DB.Content.Add(content);
                DB.SaveChanges();

                ContentBody cb = new ContentBody();
                cb.contentID = content.contentID;
                cb.code = languageCode;
                cb.lastModified = DateTime.Now;
                cb.lastModifiedByID = HttpContext.Current.User.Identity.GetUserId();
                cb.body = body;
                DB.ContentBody.Add(cb);
                DB.SaveChanges();

            }
            
            return true;
        }



        public void doAdminControls(string code)
        {
            using (ApplicationDbContext DB = new ApplicationDbContext())
            {
                foreach (Content c in content)
                {
                    if (HttpContext.Current.User.Identity.IsAuthenticated && (HttpContext.Current.User.IsInRole("editor") || HttpContext.Current.User.IsInRole("admin")))
                    {
                        var userID = HttpContext.Current.User.Identity.GetUserId();

                        if (HttpContext.Current.User.IsInRole("admin"))
                        {
                            c.addPageAdminControls();
                        }
                        else if (DB.UserLanguages.Any(ul => ul.userID == userID && ul.code == code))
                        {
                            c.addPageAdminControls();
                        }
                    }
                }
                
            }
        }


    }
}