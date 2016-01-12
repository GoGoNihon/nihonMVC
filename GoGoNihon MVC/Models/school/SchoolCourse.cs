using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity;

namespace GoGoNihon_MVC.Models
{
    public class SchoolCourse
    {

        public SchoolCourse()
        {

            languageCode = "en";
        }


        [Key]
        [Required]
        public int id { get; set; }
        public string languageCode { get; set; }
        public string introductionImage { get; set; }
        public int type { get; set; }
        
        //schedule
        public int hoursPerWeek { get; set; }

        //term
        public virtual ICollection<TermBreakdownStep> termSteps { get; set; }

        //features
        public CourseFeatures features { get; set; }


        //demography
        public int totalStudents { get; set; }
        public int studentsPerClass { get; set; }
        public string demographyImage { get; set; }
        public virtual ICollection<CourseDemography> courseDemographies { get; set; }

        [Required]
        public int schoolID { get; set; }

        [ForeignKey("schoolID")]
        public virtual School school{ get; set; }
        public List<CourseLength> courseLengths { get; set; }
        public List<Content> courseContent { get; set; }


       


        public bool addContent(ApplicationDbContext DB, string name, string body)
        {

            if (DB.Content.Where(c => c.courseID == id && c.name == name).Any())
            {
                Content c = DB.Content.Where(c2 => c2.courseID == id && c2.name == name).FirstOrDefault();

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
                content.courseID = id;
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


        public void loadContent(ApplicationDbContext DB, bool widthAdminControls = false)
        {
            courseContent = DB.Content.Include("contentCollection").Where(c => c.courseID == this.id).ToList();

            foreach (Content item in courseContent)
            {
                item.loadContent(languageCode);
                if (widthAdminControls)
                {
                    if (HttpContext.Current.User.Identity.IsAuthenticated && (HttpContext.Current.User.IsInRole("editor") || HttpContext.Current.User.IsInRole("admin")))
                    {
                        var userID = HttpContext.Current.User.Identity.GetUserId();

                        if (HttpContext.Current.User.IsInRole("admin"))
                        {
                            item.addPageAdminControls();
                        }
                        else if (DB.UserLanguages.Any(ul => ul.userID == userID && ul.code == languageCode))
                        {
                            item.addPageAdminControls();
                        }

                    }
                }
                    



                
                
            }

        }


    }
}