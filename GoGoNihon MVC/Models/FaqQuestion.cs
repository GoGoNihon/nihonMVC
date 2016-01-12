using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoGoNihon_MVC.Models
{
    public class FaqQuestion
    {
        public int id { get; set; }
        public string languageCode { get; set; }
        public List<Content> content { get; set; }

        public FaqQuestion() {
            languageCode = "en";
            content = new List<Content>();
        }


        public void udate(ApplicationDbContext DB, string question, string answer)
        {
            updateQuestion(DB, question);
            updateAnswer(DB, answer);

        }


        public void updateQuestion(ApplicationDbContext DB, string text)
        {
            addContent(DB, "question", text);
        }

        public void updateAnswer(ApplicationDbContext DB, string text)
        {
            addContent(DB, "answer", text);
        }


        protected bool addContent(ApplicationDbContext DB, string name, string body)
        {
            if(content.Count < 1)
            {
                loadContent(DB);
            }


            if (content.Where(c => c.name == name).Any())
            {
                Content con = content.Where(c => c.name == name).FirstOrDefault();

                if (con.contentCollection.Where(c => c.code == languageCode).Any())
                {
                    ContentBody cb = con.contentCollection.Where(c => c.code == languageCode).FirstOrDefault();

                    cb.body = body;
                    cb.lastModified = DateTime.Now;
                    cb.lastModifiedByID = HttpContext.Current.User.Identity.GetUserId();
                    DB.SaveChanges();
                }
                else
                {
                    ContentBody cb = new ContentBody();
                    cb.contentID = con.contentID;
                    cb.code = languageCode;
                    cb.lastModified = DateTime.Now;
                    cb.lastModifiedByID = HttpContext.Current.User.Identity.GetUserId();
                    cb.body = body;
                    con.contentCollection.Add(cb);
                    DB.SaveChanges();
                }
                
            }
            else {

                Content con = new Content();
                con.name = name;
                con.contentCollection = new List<ContentBody>();

                ContentBody cb = new ContentBody();
                cb.code = languageCode;
                cb.lastModified = DateTime.Now;
                cb.lastModifiedByID = HttpContext.Current.User.Identity.GetUserId();
                cb.body = body;
                con.contentCollection.Add(cb);

                content.Add(con);
                DB.SaveChanges();

                

            }

            return true;
        }


        public void loadContent(ApplicationDbContext DB)
        {

            if(DB.faqQuestions.Include("content.contentCollection").Where(f => f.id == id).Any())
            {
                FaqQuestion fq = DB.faqQuestions.Include("content.contentCollection").Where(f => f.id == id).FirstOrDefault();
                content = fq.content;
            }
            

            //content = (from s in DB.Content
            //           where s.galleryImageID == this.id
            //           select new { s.contentCollection, s.name, s.galleryImageID, s.contentID }).ToList()
            //                     .Select(s => new Content { contentCollection = s.contentCollection, contentID = s.contentID, name = s.name, galleryImageID = s.galleryImageID }).ToList<Content>();

            //foreach (var item in content)
            //{
            //    item.loadContent(languageCode);
            //}

        }



    }
}