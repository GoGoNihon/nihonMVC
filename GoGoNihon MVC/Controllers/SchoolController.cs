using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoGoNihon_MVC.Models;
using System.Data.Entity;


namespace GoGoNihon_MVC.Controllers
{
    public class SchoolController : Controller
    {
        // GET: School
        public ActionResult Index(string languageCode, string location, string url)
        {
            pageReturnData data = new pageReturnData();
            data.languageCode = languageCode;

            using (ApplicationDbContext DB = new ApplicationDbContext())
            {
                try
                {
                    Page page = new Page();
                    data.page = page.getPage(7, languageCode);
                    /// maybe add location, certianly add duplicate name check for schools edit and creation
                    var s = DB.Schools.FirstOrDefault(ss => ss.url == url);

                    if(s != null)
                    {
                        s.languageCode = languageCode;
                        s.loadContent(DB, true);
                        data.school = s;
                        data.courses = Functions.getSchoolCourses(s.id, languageCode, true);
                    }

                    return View("school", data);
                }
                catch (Exception ex)
                {
                    data.message = "Error: " + ex.Message;
                    return View("school", data);
                }

            }



        }
    }
}