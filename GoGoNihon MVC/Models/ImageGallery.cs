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
    public class ImageGallery
    {
        [Required]
        [Key]
        public int id { get; set; }

        public string languageCode { get; set; }

        public virtual List<Content> content { get; set; }
        public virtual ICollection<GalleryImage> galleryImages { get; set; }

        public ImageGallery()
        {
            languageCode = "en";
            content = new List<Content>();
            galleryImages = new List<GalleryImage>();
        }

        public void addTitle(ApplicationDbContext DB, string name)
        {
            addContent(DB, "name", name);
        }
        

        public bool addImage(int galleryID, string title, string text, HttpPostedFileWrapper image, string tag)
        {

            try {

                using (ApplicationDbContext DB = new ApplicationDbContext())
                {

                    ImageGallery gal = DB.gallerys.Where(g => g.id == id).FirstOrDefault();
                    GalleryImage gi = new GalleryImage();
                    gi.galleryID = galleryID;
                    gi.tag = tag;
                    gal.galleryImages.Add(gi);
                    DB.SaveChanges();

                    if (image != null && image.ContentLength > 0)
                    {

                        string path = "/content/images/uploads/galleries/" + galleryID;
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

                    gi.addText(DB, text);
                    gi.addTitle(DB, title);



                }

                return true;


            }
            catch(Exception)
            {

                return false;
            }


            
        }

        protected bool addContent(ApplicationDbContext DB, string name, string body)
        {
            

            if (DB.Content.Where(c => c.galleryID == id && c.name == name).Any())
            {
                Content c = DB.Content.Where(c2 => c2.galleryID == id && c2.name == name).FirstOrDefault();

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
                content.galleryID = id;
                content.name = name;
                ImageGallery imageGal = DB.gallerys.Where(i => i.id == id).FirstOrDefault();
                imageGal.content.Add(content);
                //DB.Content.Add(content);
                DB.SaveChanges();

                ContentBody cb = new ContentBody();
                cb.contentID = content.contentID;
                cb.code = languageCode;
                cb.lastModified = DateTime.Now;
                cb.lastModifiedByID = HttpContext.Current.User.Identity.GetUserId();
                cb.body = body;
                content.contentCollection.Add(cb);
                //DB.ContentBody.Add(cb);
                DB.SaveChanges();

            }

            return true;
        }


        public void loadContent(ApplicationDbContext DB)
        {
            content = (from s in DB.Content
                       where s.galleryID == this.id
                       select new { s.contentCollection, s.name, s.galleryID, s.contentID }).ToList()
                                 .Select(s => new Content { contentCollection = s.contentCollection, contentID = s.contentID, name = s.name, galleryID = s.galleryID }).ToList<Content>();

            foreach (Content item in content)
            {
                item.loadContent("en");
            }


            galleryImages = (from i in DB.galleryImages
                       where i.galleryID == this.id
                       select new { i.id, i.content,i.galleryID,i.image,i.languageCode }).ToList()
                                 .Select(i => new GalleryImage { content = i.content, galleryID = i.galleryID, id = i.id, image = i.image, languageCode = i.languageCode  }).ToList<GalleryImage>();

            foreach (GalleryImage item in galleryImages)
            {
                item.loadContent(DB);
            }

        }



        
    }
}