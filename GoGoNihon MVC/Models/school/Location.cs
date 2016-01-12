using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GoGoNihon_MVC.Models
{

    public struct locationContent
    {
        public string name;
        public string body;
        public int contentBodyID;
        public int contentID;
    }

    public class Location
    {
        [Key]
        [Required]
        public int id { get; set; }
        
        public string image { get; set; }
        public string name { get; set; }
        public string city { get; set; }

        public string tempCode { get; set; }
        public List<locationContent> content { get; set; }
        public virtual List<Content> contentCollection { get; set; }

        public virtual ImageGallery locationGallery { get; set; }


        public Location()
        {
            tempCode = "en";
        }



        public void loadContent(string code)
        {
            this.tempCode = code;
            using (ApplicationDbContext DB = new ApplicationDbContext())
            {
                content = new List<locationContent>();
                contentCollection = DB.Content.Where(c => c.locationID == id).ToList()
                .Select(c => new Content {name = c.name, contentCollection = c.contentCollection, contentID = c.contentID, locationID = c.locationID }).ToList<Content>();
                
                foreach (var c in contentCollection)
                {
                    c.loadContent(code);

                    foreach (var item in c.contentCollection)
                    {
                        locationContent lc = new locationContent();
                        lc.name = c.name;
                        lc.body = item.body;
                        lc.contentID = item.contentID;
                        lc.contentBodyID = item.contentBodyID;
                        content.Add(lc);
                    }

                    
                }
                doAdminControls(code);
            }
        }

        public void doAdminControls(string code)
        {
            using (ApplicationDbContext DB = new ApplicationDbContext())
            {
                foreach (Content c in contentCollection)
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

        public bool addContent(ApplicationDbContext DB, string name, string body)
        {

            if (DB.Content.Where(c => c.locationID == id && c.name == name).Any())
            {
                Content c = DB.Content.Where(c2 => c2.locationID == id && c2.name == name).FirstOrDefault();

                if (DB.ContentBody.Where(cb2 => cb2.contentID == c.contentID && cb2.code == tempCode).Any())
                {
                    ContentBody cb3 = DB.ContentBody.Where(cb2 => cb2.contentID == c.contentID && cb2.code == tempCode).FirstOrDefault();
                    cb3.body = body;
                    cb3.lastModified = DateTime.Now;
                    cb3.lastModifiedByID = HttpContext.Current.User.Identity.GetUserId();
                    DB.SaveChanges();
                }
                else
                {
                    ContentBody cb3 = new ContentBody();
                    cb3.contentID = c.contentID;
                    cb3.code = tempCode;
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
                content.locationID = id;
                content.name = name;
                DB.Content.Add(content);
                DB.SaveChanges();

                ContentBody cb = new ContentBody();
                cb.contentID = content.contentID;
                cb.code = tempCode;
                cb.lastModified = DateTime.Now;
                cb.lastModifiedByID = HttpContext.Current.User.Identity.GetUserId();
                cb.body = body;
                DB.ContentBody.Add(cb);
                DB.SaveChanges();
            }
            
            return true;
        }



    }
}