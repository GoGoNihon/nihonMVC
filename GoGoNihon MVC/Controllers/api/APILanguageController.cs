using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoGoNihon_MVC.Models;
using System.IO;
using System.Data.Entity;

namespace GoGoNihon_MVC.Controllers
{
    [Authorize(Roles = "admin")]
    public class APILanguageController : Controller
    {


        [Route("api/getLanguage/{code}")]
        public ActionResult getLanguage(string code)
        {
            ajaxReturnData data = new ajaxReturnData();

            using (ApplicationDbContext DB = new ApplicationDbContext())
            {
                try
                {
                    
                    data.data = (from l in DB.Language
                                   where l.code == code
                                             select new { l.code, l.flag, l.name }).ToList(); ;

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



        [Route("api/addLanguage")]
        public ActionResult addLanguage(HttpPostedFileBase flag, string name, string code)
        {
            ajaxReturnData data = new ajaxReturnData();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                try
                {

                    string path = Server.MapPath("/content/images/");
                    path += flag.FileName;
                    flag.SaveAs(Server.MapPath("~/content/images/flag-" + name + Path.GetExtension(flag.FileName)));
                    Language language = new Language();
                    language.flag = "/content/images/flag-" + name + Path.GetExtension(flag.FileName);
                    language.name = name;
                    language.code = code;

                    db.Language.Add(language);
                    db.SaveChanges();
                    data.statusCode = (int)statusCodes.successRun;
                    data.callback = "getLanguageList";
                    data.message = "Language \"" + name + "\" added";
                }
                catch (Exception ex)
                {
                    data.statusCode = (int)statusCodes.fail;
                    data.message = "Error: Language not added; " + ex.Message;
                }
            }


            return Json(data);
        }


        [Route("api/editLanguage/{callback?}")]
        public ActionResult editLanguage(HttpPostedFileBase flag, string name, string code, string callback)
        {
            ajaxReturnData data = new ajaxReturnData();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                try
                {

                    Language language = new Language();

                    
                    language.name = name;
                    language.code = code;

                    db.Language.Attach(language);

                    db.Entry(language).Property(f => f.name).IsModified = true;
                    db.Entry(language).Property(f => f.code).IsModified = false;

                    if (flag != null)
                    {
                        string path = Server.MapPath("/content/images/");
                        path += flag.FileName;
                        flag.SaveAs(Server.MapPath("~/content/images/flag-" + name + Path.GetExtension(flag.FileName)));
                        language.flag = "/content/images/flag-" + name + Path.GetExtension(flag.FileName);
                        db.Entry(language).Property(f => f.flag).IsModified = true;
                    }
                    else
                    {
                        db.Entry(language).Property(f => f.flag).IsModified = false;
                    }
                    
                    
                    db.SaveChanges();

                    data.message = "Language \"" + name + "\" updated";

                    if (String.IsNullOrEmpty(callback))
                    {
                        data.statusCode = (int)statusCodes.success;
                    }
                    else
                    {
                        data.statusCode = (int)statusCodes.successRun;
                        data.callback = callback;
                    }

                    
                }
                catch (Exception ex)
                {
                    data.statusCode = (int)statusCodes.fail;
                    data.message = "Error: Language not updated; " + ex.Message;
                }
            }


            return Json(data);
        }

        [Route("api/getLanguageList/{callback?}")]
        public ActionResult getLanguageList(string callback)
        {
            ajaxReturnData data = new ajaxReturnData();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                try
                {
                    data.data = db.Language.ToList<Language>();

                    if (String.IsNullOrEmpty(callback))
                    {
                        data.statusCode = (int)statusCodes.success;
                    }
                    else
                    {
                        data.statusCode = (int)statusCodes.successRun;
                        data.callback = callback;
                    }
                   
                }
                catch (Exception ex)
                {
                    data.statusCode = (int)statusCodes.fail;
                    data.message = "Error: " + ex.Message;
                }
            }


            return Json(data);
        }

        [Authorize]
        [Route("api/deleteLanguage/{code}/{callback?}")]
        public ActionResult deleteLanguage(string code, string callback)
        {
            ajaxReturnData data = new ajaxReturnData();

            using (ApplicationDbContext DB = new ApplicationDbContext())
            {
                try
                {
                    Language language = DB.Language.Where(l => l.code == code).FirstOrDefault();
                    DB.Language.Remove(language);
                    DB.SaveChanges();
                    data.message = "language deleted";


                    if (String.IsNullOrEmpty(callback))
                    {
                        data.statusCode = (int)statusCodes.success;
                    }
                    else
                    {
                        data.statusCode = (int)statusCodes.successRun;
                        data.callback = callback;
                    }

                }
                catch (Exception ex)
                {
                    data.statusCode = (int)statusCodes.fail;
                    data.message = "Error deleting language: " + ex.Message;
                }
            }


            return Json(data);
        }




    }
}