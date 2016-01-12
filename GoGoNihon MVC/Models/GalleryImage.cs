using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Web;

namespace GoGoNihon_MVC.Models
{
    public class GalleryImage
    {
        [Key]
        public int id { get; set; }
     
        public string image { get; set; }
        
        public int galleryID { get; set; }
        public string languageCode { get; set; }
        public string tag { get; set; }

        public List<Content> content { get; set; }


        public GalleryImage()
        {

            languageCode = "en";
            content = new List<Content>();
        }


        public void udate(ApplicationDbContext DB, string title, string text, HttpPostedFileWrapper image, string tag)
        {
            this.tag = tag;
            addImage(DB, image);
            addContent(DB, "title", title);
            addContent(DB, "text", text);
            
        }



        public void addImage(ApplicationDbContext DB, HttpPostedFileWrapper image) {

            if(DB.galleryImages.Where(g => g.id == id).Any())
            {

                GalleryImage gi = DB.galleryImages.Where(g => g.id == id).FirstOrDefault();
                

                if (image != null && image.ContentLength > 0)
                {

                    string path = "/content/images/uploads/galleries/" + gi.galleryID;
                    bool exists = Directory.Exists(HttpContext.Current.Server.MapPath(path));
                    if (!exists)
                    {
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath(path));
                    }

                    path = path + "/" + gi.id + Path.GetExtension(image.FileName);
                    image.SaveAs(HttpContext.Current.Server.MapPath(path));
                    gi.image = path;
                }

                DB.SaveChanges();

            }

            

        }

        public void addTitle(ApplicationDbContext DB, string title)
        {
            addContent(DB, "title", title);
        }

        public void addText(ApplicationDbContext DB, string text)
        {
            addContent(DB, "text", text);
        }


        protected bool addContent(ApplicationDbContext DB, string name, string body)
        {

            if (DB.Content.Where(c => c.galleryImageID == id && c.name == name).Any())
            {
                Content c = DB.Content.Where(c2 => c2.galleryImageID == id && c2.name == name).FirstOrDefault();

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
                content.galleryImageID = id;
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
                       where s.galleryImageID == this.id
                       select new { s.contentCollection, s.name, s.galleryImageID, s.contentID }).ToList()
                                 .Select(s => new Content { contentCollection = s.contentCollection, contentID = s.contentID, name = s.name, galleryImageID = s.galleryImageID }).ToList<Content>();

            foreach (var item in content)
            {
                item.loadContent(languageCode);
            }

        }


    }
}