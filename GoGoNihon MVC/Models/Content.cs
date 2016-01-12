using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GoGoNihon_MVC.Models
{
    public class Content
    {
        [Key]
        public int contentID { get; set; }
       
        public int? pageID { get; set; }

        
        public int? locationID { get; set; }

        public int? schoolID { get; set; }
        public int? courseID { get; set; }
        public int? termStepID { get; set; }
        public int? demographyID { get; set; }
        public int? shortCourseID { get; set; }
        public int? galleryID { get; set; }
        public int? galleryImageID { get; set; }
        public int? planFeatureID { get; set; }


        [Required]
        public string name { get; set; }

        
        public ICollection<ContentBody> contentCollection { get; set; }
        public virtual ICollection<Language> languageCollection { get; set; }


        public Content()
        {
            contentCollection = new List<ContentBody>();
        }

        public void loadContent(string languageCode)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (!String.IsNullOrWhiteSpace(languageCode))
                {
                    if (languageCode != "*")
                    {
                        contentCollection = (from cb in db.ContentBody
                                             where cb.contentID == contentID && cb.code == languageCode
                                             select new { contentID = cb.contentID, contentBodyID = cb.contentBodyID, code = cb.code, lastModified = cb.lastModified, lastModifiedByID = cb.lastModifiedByID, body = cb.body }).ToList()
                                 .Select(cb => new ContentBody { contentID = cb.contentID, contentBodyID = cb.contentBodyID, code = cb.code, lastModified = cb.lastModified, lastModifiedByID = cb.lastModifiedByID, body = cb.body }).ToList<ContentBody>();
                        
                    }
                    else
                    {

                        contentCollection = (from cb in db.ContentBody
                                             where cb.contentID == contentID
                                             select new { contentID = cb.contentID, contentBodyID = cb.contentBodyID, code = cb.code, lastModified = cb.lastModified, lastModifiedByID = cb.lastModifiedByID, body = cb.body }).ToList()
                                 .Select(cb => new ContentBody { contentID = cb.contentID, contentBodyID = cb.contentBodyID, code = cb.code, lastModified = cb.lastModified, lastModifiedByID = cb.lastModifiedByID, body = cb.body }).ToList<ContentBody>();
                        
                    }

                    checkContent(languageCode);

                }
            }

        }
        public void checkContent(string languageCode)
        {

            using (ApplicationDbContext DB = new ApplicationDbContext())
            {
                if (languageCode != "*")
                {

                    if(contentCollection == null || contentCollection.Count < 1)
                    {
                        
                        ContentBody cb = new ContentBody();
                        cb.code = languageCode;
                        cb.body = "content not found";
                        contentCollection.Add(cb);
                    }
                }
                else
                {
                    foreach (Language language in DB.Language.ToList<Language>())
                    {
                        bool languageFound = false;
                        foreach (ContentBody cb in contentCollection)
                        {
                            if (cb.code == language.code)
                            {
                                languageFound = true;
                            }
                        }

                        if (!languageFound)
                        {
                            ContentBody cb = new ContentBody();
                            cb.code = language.code;
                            cb.body = "content not found";
                            contentCollection.Add(cb);
                        }

                    }

                }

                

            }



        }


     


        public void addPageAdminControls()
        {
            foreach (ContentBody cb in contentCollection)
            {
                if(cb.body == "content not found")
                {
                    cb.body = "<div class='editable'>content not found";
                    cb.body += "<div class='edit-region noData-click'>";
                    cb.body += "<div class='getContentBody'>";
                    cb.body += "<span class='icon-pencil' data-displayEditNoData data-contentID='" + contentID + "' data-name='" + name + "' data-code='" + cb.code + "'  ></span></div></div>";
                    cb.body += "</div>";
                     
                }
                else
                {
                    cb.body = "<div class='editable'>" + cb.body; 
                    cb.body += "<div class='edit-click edit-region'>";
                    cb.body += "<form class='getContentBody' action='/api/getContentBody/" + cb.contentBodyID + "/displayEditModal' method='post' data-ajax>";
                    cb.body += "<span class='icon-pencil'></span></form></div></div>";
                }


                
            }

        }

    }
}