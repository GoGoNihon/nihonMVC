using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoGoNihon_MVC.Models;
using System.IO;
using System.Data.Entity;

namespace GoGoNihon_MVC.Models
{
    struct ajaxReturnData
    {
        public int statusCode;
        public string message;
        public string callback;  //if you need to run a JS function after the ajax request.
        public object data;

    }


    enum statusCodes
    {
        success,
        successRun, //success with requirement to run a JS function after
        fail,
        failRun
    };

}


namespace GoGoNihon_MVC.Controllers
{
    [Authorize(Roles = "admin")]
    public class APIPageController : Controller
    {
        
        [Route("api/addNewPage")]
        public ActionResult AddNewPage(Page page)
        {
            ajaxReturnData data = new ajaxReturnData();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                
                try
                {
                    db.Pages.Add(page);
                    db.SaveChanges();
                    data.statusCode = (int)statusCodes.successRun;
                    data.callback = "getPageList";
                    data.message = "Page added";
                }
                catch (Exception ex)
                {
                    data.statusCode = (int)statusCodes.fail;
                    data.message = "Error: " + ex.Message;
                }
                
            }
            

                return Json(data);
        }



        [Route("api/getPage/{pageID}")]
        public ActionResult getPage(int pageID)
        {
            ajaxReturnData data = new ajaxReturnData();

            using (ApplicationDbContext DB = new ApplicationDbContext())
            {

                try
                {

                    data.data = DB.Pages.Where(p=>p.pageID == pageID).FirstOrDefault();
                    data.statusCode = (int)statusCodes.success;
                }
                catch (Exception ex)
                {
                    data.statusCode = (int)statusCodes.fail;
                    data.message = "Error: " + ex.Message;
                }

            }


            return Json(data);
        }


        [Route("api/editPage/{callback?}")]
        public ActionResult editPage(Page page, string callback)
        {
            ajaxReturnData data = new ajaxReturnData();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {

                try
                {
                    //db.Pages.Attach(page);


                    db.Entry(page).State = EntityState.Modified;
                    db.SaveChanges();

                    if (string.IsNullOrEmpty(callback))
                    {
                        data.statusCode = (int)statusCodes.success;
                    }
                    else
                    {
                        data.statusCode = (int)statusCodes.successRun;
                        data.callback = callback;
                    }


                    data.message = "Page updated";
                }
                catch (Exception ex)
                {
                    data.statusCode = (int)statusCodes.fail;
                    data.message = "Error: " + ex.Message;
                }

            }


            return Json(data);
        }


        


        [Route("api/deletePage/{pageID}")]
        public ActionResult deletePage(int pageID)
        {
            ajaxReturnData data = new ajaxReturnData();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                try
                {
                    Page doomedPage = new Page();
                    doomedPage.pageID = pageID;

                    db.Pages.Attach(doomedPage);
                    db.Pages.Remove(doomedPage);
                    db.SaveChanges();
                    data.message = "Page removed";
                    data.statusCode = (int)statusCodes.successRun;
                    data.callback = "getPageList";
                }
                catch (Exception ex)
                {
                    data.statusCode = (int)statusCodes.fail;
                    data.message = "Error: " + ex.Message;
                }

            }


            return Json(data);
        }


        [Route("api/getPageList")]
        public ActionResult getPageList()
        {
            ajaxReturnData data = new ajaxReturnData();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                try
                {
                    data.data = db.Pages.ToList();
                    data.statusCode = (int)statusCodes.successRun;
                    data.callback = "fillPageList";
                }
                catch (Exception ex)
                {
                    data.statusCode = (int)statusCodes.fail;
                    data.message = "Error: " + ex.Message;
                }
            }


            return Json(data);
        }


        [Route("api/addPageGallery/{pageID}/{callback?}")]
        public ActionResult addPageGallery(int pageID, string callback, string name)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    ImageGallery gal = new ImageGallery();
                    DB.gallerys.Add(gal);
                    DB.SaveChanges();
                    gal.addTitle(DB, name);

                    Page page = DB.Pages.Where(p => p.pageID == pageID).Include("galleries").FirstOrDefault();
                    page.galleries.Add(gal);
                    DB.SaveChanges();
                }


                if (string.IsNullOrEmpty(callback))
                {
                    data.statusCode = (int)statusCodes.success;
                }
                else
                {
                    data.statusCode = (int)statusCodes.successRun;
                    data.callback = callback;
                }

                data.message = "page gallery added";
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "failed to add page gallery; " + ex.Message;
                return Json(data);
            }
        }


        [Route("api/getPageContent/{pageID}/{callback?}")]
        public ActionResult getPageContent(int pageID, string callback, string name)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    
                    Page page = DB.Pages.Where(p => p.pageID == pageID).Include("galleries.galleryImages").Include("galleries.content.contentCollection").Include("faqs.questions.content.contentCollection").Include("content.contentCollection").FirstOrDefault();
                    data.data = page;
                }


                if (string.IsNullOrEmpty(callback))
                {
                    data.statusCode = (int)statusCodes.success;
                }
                else
                {
                    data.statusCode = (int)statusCodes.successRun;
                    data.callback = callback;
                }
                
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "failed to get page content; " + ex.Message;
                return Json(data);
            }
        }


        [Authorize]
        [HttpPost]
        [Route("api/deletePageGallery/{pageID}/{galleryID}/{callback?}")]
        public ActionResult deleteGallery(int pageID, int galleryID, string callback)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    Page page = DB.Pages.Include("galleries").Where(p => p.pageID == pageID).FirstOrDefault();

                    ImageGallery gal = page.galleries.Where(g => g.id == galleryID).FirstOrDefault();
                    page.galleries.Remove(gal);
                    DB.SaveChanges();

                    data.message = "gallery deleted";
                    if (string.IsNullOrEmpty(callback))
                    {
                        data.statusCode = (int)statusCodes.success;
                    }
                    else
                    {
                        data.statusCode = (int)statusCodes.successRun;
                        data.callback = callback;
                    }
                }
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to delete gallery; " + ex.Message;
                return Json(data);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/addPageFaq/{pageID}/{callback?}")]
        public ActionResult addPageFaq(int pageID, string name, string callback)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    Page page = DB.Pages.Include("faqs").Where(p => p.pageID == pageID).FirstOrDefault();

                    Faq faq = new Faq();
                    faq.name = name;
                    page.faqs.Add(faq);
                    DB.SaveChanges();

                    data.message = "faq added";

                    if (string.IsNullOrEmpty(callback))
                    {
                        data.statusCode = (int)statusCodes.success;
                    }
                    else
                    {
                        data.statusCode = (int)statusCodes.successRun;
                        data.callback = callback;
                    }
                }
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to add faq; " + ex.Message;
                return Json(data);
            }
        }

        [ValidateInput(false)]
        [HttpPost]
        [Route("api/addPageQuestion/{pageID}/{faqID}/{callback?}")]
        public ActionResult addPageQuestion(string callback, int pageID, int faqID, string question, string answer)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    Page page = DB.Pages.Include("faqs.questions").Where(p => p.pageID == pageID).FirstOrDefault();
                    Faq faq = page.faqs.Where(f => f.id == faqID).FirstOrDefault();
                    FaqQuestion fq = new FaqQuestion();
                    fq.udate(DB, question, answer);
                    faq.questions.Add(fq);
                    DB.SaveChanges();

                }


                if (string.IsNullOrEmpty(callback))
                {
                    data.statusCode = (int)statusCodes.success;
                }
                else
                {
                    data.statusCode = (int)statusCodes.successRun;
                    data.callback = callback;
                }

                //data.message = "question added";
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to add question; " + ex.Message;
                return Json(data);
            }
        }


       
        [HttpPost]
        [Route("api/deletePageFaq/{pageID}/{faqID}/{callback?}")]
        public ActionResult deleteFaq(string callback, int pageID, int faqID)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    Page page = DB.Pages.Include("faqs").Where(p => p.pageID == pageID).FirstOrDefault();
                    Faq f = DB.faqs.Where(fq => fq.id == faqID).FirstOrDefault();
                    page.faqs.Remove(f);
                    DB.SaveChanges();

                }


                if (string.IsNullOrEmpty(callback))
                {
                    data.statusCode = (int)statusCodes.success;
                }
                else
                {
                    data.statusCode = (int)statusCodes.successRun;
                    data.callback = callback;
                }

                data.message = "faq deleted from page";
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to delete faq; " + ex.Message;
                return Json(data);
            }
        }


    }
}