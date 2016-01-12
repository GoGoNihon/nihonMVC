using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity;

namespace GoGoNihon_MVC.Models
{
    public class CourseDemography
    {
        [Key]
        [Required]
        public int id { get; set; }
        [Required]
        public int percent { get; set; }

        public int schoolCourseID { get; set; }

        public string languageCode { get; set; }

        public List<Content> content { get; set; }

        [ForeignKey("schoolCourseID")]
        public virtual SchoolCourse schoolCourse { get; set; }

        public CourseDemography() {

            languageCode = "en";
        }


        public bool addContent(ApplicationDbContext DB, string name, string body)
        {

            if (DB.Content.Where(c => c.demographyID == id && c.name == name).Any())
            {
                Content c = DB.Content.Where(c2 => c2.demographyID == id && c2.name == name).FirstOrDefault();

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
                content.demographyID = id;
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
            content = (from s in DB.Content
                             where s.demographyID == this.id
                             select new { s.contentCollection, s.name, s.schoolID, s.contentID }).ToList()
                                 .Select(s => new Content { contentCollection = s.contentCollection, contentID = s.contentID, name = s.name, schoolID = s.schoolID }).ToList<Content>();

            foreach (var item in content)
            {
                item.loadContent(languageCode);
            }

        }



    }
}