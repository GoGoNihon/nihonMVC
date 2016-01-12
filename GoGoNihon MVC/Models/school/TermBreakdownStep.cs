using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity;

namespace GoGoNihon_MVC.Models
{
    public class TermBreakdownStep
    {
        [Key]
        [Required]
        public int id { get; set; }
        [Required]
        public int index { get; set; }

        //[Required]
        //public int schoolCourseID { get; set; }

        //[ForeignKey("schoolCourseID")]
        //private  SchoolCourse schoolCourse {  get; set; }

        public string languageCode { get; set; }

        public List<Content> stepContent { get; set; } 

        public TermBreakdownStep()
        {

            languageCode = "en";
        }

        public bool addContent(ApplicationDbContext DB, string name, string body)
        {

            if (DB.Content.Where(c => c.termStepID == id && c.name == name).Any())
            {
                Content c = DB.Content.Where(c2 => c2.termStepID == id && c2.name == name).FirstOrDefault();

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
                content.termStepID = id;
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

        public void loadContent(ApplicationDbContext DB)
        {
            stepContent = (from s in DB.Content
                             where s.termStepID == this.id
                             select new { s.contentCollection, s.name, s.termStepID, s.contentID }).ToList()
                                 .Select(s => new Content { contentCollection = s.contentCollection, contentID = s.contentID, name = s.name, termStepID = s.termStepID }).ToList<Content>();

            foreach (var item in stepContent)
            {
                item.loadContent(languageCode);
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