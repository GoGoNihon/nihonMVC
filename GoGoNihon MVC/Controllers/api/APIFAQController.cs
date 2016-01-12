using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoGoNihon_MVC.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;

namespace GoGoNihon_MVC.Controllers
{
    public class APIFAQController : Controller
    {
        //[HttpPost]
        //[Route("api/addFaq/{callback?}")]
        //public ActionResult addFaq(string callback, string languageCode = "en")
        //{
        //    ajaxReturnData data = new ajaxReturnData();
        //    ShortCourse course = new ShortCourse();

        //    try
        //    {
        //        using (ApplicationDbContext DB = new ApplicationDbContext())
        //        {
        //            Faq faq = new Faq();
        //            //FaqQuestion fq = new FaqQuestion();
        //            //fq.udate(DB, "does this work?", "i hightly doubt it");
        //            //faq.questions.Add(fq);
        //            //DB.faqs.Add(faq);
        //            DB.SaveChanges();
        //        }

        //        if (string.IsNullOrEmpty(callback))
        //        {
        //            data.statusCode = (int)statusCodes.success;
        //        }
        //        else
        //        {
        //            data.statusCode = (int)statusCodes.successRun;
        //            data.callback = callback;
        //        }

        //        data.message = "FAQ created";
        //        return Json(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        data.statusCode = (int)statusCodes.fail;
        //        data.message = "Failed to create FAQ; " + ex.Message;
        //        return Json(data);
        //    }
        //}


        [ValidateInput(false)]
        [HttpPost]
        [Route("api/editQuestion/{id}/{callback?}")]
        public ActionResult editQuestion(string callback, int id, string question, string answer)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    FaqQuestion q = DB.faqQuestions.Where(fq => fq.id == id).FirstOrDefault();
                    q.udate(DB, question, answer);
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

                data.message = "question updated";
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to update question; " + ex.Message;
                return Json(data);
            }
        }

        [ValidateInput(false)]
        [HttpPost]
        [Route("api/deleteQuestion/{id}/{callback?}")]
        public ActionResult deleteQuestion(string callback, int id)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    FaqQuestion q = DB.faqQuestions.Where(fq => fq.id == id).FirstOrDefault();
                    DB.faqQuestions.Remove(q);
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

                //data.message = "question sentenced to the very depths of hell";
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to delete question; " + ex.Message;
                return Json(data);
            }
        }


        [Authorize]
        [HttpPost]
        [Route("api/getQuestions/{id}/{callback?}")]
        public ActionResult getQuestions(int id, string callback)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {

                    Faq f = DB.faqs.Include("questions.content.contentCollection").Where(fa => fa.id == id).FirstOrDefault();
                    data.data = f.questions;
                    

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
                data.message = "Failed to get questions; " + ex.Message;
                return Json(data);
            }
        }

        

    }
}