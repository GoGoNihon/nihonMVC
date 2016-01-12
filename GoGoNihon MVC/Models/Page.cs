using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace GoGoNihon_MVC.Models
{
    public class Page
    {
        [Key]
        public int pageID { get; set; }
        [Required]
        public string name { get; set; }
        public string description { get; set; }

        public ICollection<Content> content { get; set; }
        public ICollection<ImageGallery> galleries { get; set; }
        public ICollection<Faq> faqs { get; set; }

        public Page() {

            galleries = new List<ImageGallery>();
            faqs = new List<Faq>();
        }



        public Page getPage(int pageID, string languageCode, bool withAdminControls=true)
        {
            Page page = new Page();
            using (ApplicationDbContext DB = new ApplicationDbContext()) { 

                page = DB.Pages
                        .Include("galleries.galleryImages.content")
                        .Include("galleries.content")
                        .Include("faqs.questions.content")
                        .Include("content")
                        .Where(p => p.pageID == pageID).FirstOrDefault();
                }

            page.getContentByLanguage(languageCode);

            if (withAdminControls)
            {
                page.doAdminControls(languageCode);
            }

            return page;
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

                foreach (Faq f in faqs)
                {
                    foreach (FaqQuestion q in f.questions)
                    {
                        foreach (Content c in q.content)
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


                foreach (ImageGallery ig in galleries)
                {
                    foreach (Content c in ig.content)
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


                    foreach (GalleryImage img in ig.galleryImages)
                    {
                        foreach (Content c in img.content)
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

        public void getContentByLanguage(string code)
        {
            using (ApplicationDbContext DB = new ApplicationDbContext())
            {
                content = DB.Content.Where(c => c.pageID == pageID || c.pageID == 1).ToList();
                foreach (Content c in content)
                {
                    c.loadContent(code);
                }

                foreach (Faq f in faqs)
                {
                    foreach (FaqQuestion q in f.questions)
                    {
                        foreach (Content c in q.content)
                        {
                            c.loadContent(code);
                        }
                    }
                    
                }

                foreach (ImageGallery g in galleries)
                {
                    foreach (Content c in g.content)
                    {
                        c.loadContent(code);
                    }

                    foreach (GalleryImage img in g.galleryImages)
                    {
                        foreach (Content c in img.content)
                        {
                            c.loadContent(code);
                        }
                    }

                }

            }
        }

        //public void getDisplayContent(string languageCode)
        //{
        //    if(pageID > 0)
        //    {
        //        using (ApplicationDbContext DB = new ApplicationDbContext())
        //        {

        //            foreach (Content c in content)
        //            {
                        
                        


        //                if (HttpContext.Current.User.Identity.IsAuthenticated && (HttpContext.Current.User.IsInRole("editor") || HttpContext.Current.User.IsInRole("admin")))
        //                {
        //                    var userID = HttpContext.Current.User.Identity.GetUserId();

        //                    if (HttpContext.Current.User.IsInRole("admin"))
        //                    {
        //                        c.addPageAdminControls();
        //                    }
        //                    else if (DB.UserLanguages.Any(ul => ul.userID == userID && ul.code == languageCode))
        //                    {
        //                        c.addPageAdminControls();
        //                    }


        //                }

        //                content.Add(c);
        //            }

        //        }
                

        //    }
        //    //using (ApplicationDbContext db = new ApplicationDbContext())
        //    //{
        //    //    var currentPage = db.Pages.Where(p=>p.pageID == pageID).t

        //    //    foreach (var element in currentPage)
        //    //    {
        //    //        pageID = Convert.ToInt16(element.pageID);
        //    //        name = element.name;
        //    //        description = element.description;
        //    //    }
        //    //}
        //    //pageContent = new List<Content>();
        //    //loadSubContent(pageID, languageCode);

        //}


        //public void loadSubContent(int pageID, string languageCode)
        //{
        //    using (ApplicationDbContext db = new ApplicationDbContext())
        //    {
        //        var subContent = (from c in db.Content 
        //                          where c.pageID == pageID || c.pageID == 2
        //                          select new {c.name,c.contentID}).ToList();

        //        foreach (var element in subContent)
        //        {
        //            Content c = new Content();
        //            c.name = element.name;
        //            c.pageID = pageID;
        //            c.contentID = element.contentID;
        //            c.loadContent(languageCode);

                    
                    
        //            if (HttpContext.Current.User.Identity.IsAuthenticated && (HttpContext.Current.User.IsInRole("editor") || HttpContext.Current.User.IsInRole("admin")))
        //            {
        //                var userID = HttpContext.Current.User.Identity.GetUserId();

        //                if (HttpContext.Current.User.IsInRole("admin"))
        //                {
        //                    c.addPageAdminControls();
        //                }
        //                else if (db.UserLanguages.Any(ul => ul.userID == userID && ul.code == languageCode))
        //                {
        //                    c.addPageAdminControls();
        //                }

                        
        //            }

        //            content.Add(c);
        //        }
        //    }
        //}
        

        
    }
}