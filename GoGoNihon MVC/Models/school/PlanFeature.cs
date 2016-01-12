using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GoGoNihon_MVC.Models
{
    public class PlanFeature
    {
        public int id { get; set; }
        public int shortCourseID { get; set; }

        public List<Content> content { get; set; }
        public string languageCode { get; set; }

        public bool gold { get; set; }
        public bool silver { get; set; }
        public bool bronze { get; set; }

        public PlanFeature()
        {
            languageCode = "en";
        }


        public void addTitle(ApplicationDbContext DB, string name)
        {
            addContent(DB, "name", name);
        }



        protected bool addContent(ApplicationDbContext DB, string name, string body)
        {

            if (DB.Content.Where(c => c.planFeatureID == id && c.name == name).Any())
            {
                Content c = DB.Content.Where(c2 => c2.planFeatureID == id && c2.name == name).FirstOrDefault();

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
                content.planFeatureID = id;
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
                       where s.planFeatureID == this.id
                       select new { s.contentCollection, s.name, s.termStepID, s.contentID }).ToList()
                                 .Select(s => new Content { contentCollection = s.contentCollection, contentID = s.contentID, name = s.name, termStepID = s.termStepID }).ToList<Content>();

            foreach (var item in content)
            {
                item.loadContent(languageCode);
            }

        }


    }
}