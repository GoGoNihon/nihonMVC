using GoGoNihon_MVC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace GoGoNihon_MVC.Models
{
    public struct pageReturnData
    {
        public Page page;
        public School school;
        public List<School> schools;
        public List<SchoolCourse> courses;
        public string message;
        public string languageCode;
    }


}



namespace GoGoNihon_MVC.Controllers
{
   
    public class HomeController : Controller
    {


      // route homepage
        public ActionResult Index(string languageCode)
        {

            pageReturnData data = new pageReturnData();
            data.languageCode = languageCode;

            using (ApplicationDbContext DB = new ApplicationDbContext())
            {
                try
                {
                    Page page = new Page();
                    data.page = page.getPage(2, languageCode);

                }
                catch (Exception ex)
                {
                    data.message = "Error: " + ex.Message;
                }

            }

                return View("index", data);
        }



        // route japan-apartments-rent-guesthouse-homestay
        public ActionResult accomodation(string languageCode)
        {
            pageReturnData data = new pageReturnData();
            data.languageCode = languageCode;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                try
                {
                    Page page = new Page();
                    data.page = page.getPage(3, languageCode);

                }
                catch (Exception ex)
                {
                    data.message = "Error: " + ex.Message;
                }
            }
            
            return View("Accomodation", data);
        }

        // route terms-and-conditions
        public ActionResult careers(string languageCode)
        {
            pageReturnData data = new pageReturnData();
            data.languageCode = languageCode;
            data.page = new Page();

            using (ApplicationDbContext DB = new ApplicationDbContext())
            {
                try
                {
                    Page page = new Page();
                    data.page = page.getPage(5, languageCode);
                }
                catch (Exception ex)
                {
                    data.message = "Error: " + ex.Message;
                }

            }

            //return Json(data,JsonRequestBehavior.AllowGet);
            return View("careers", data);
        }

        // route terms-and-conditions
        public ActionResult terms(string languageCode)
        {
            pageReturnData data = new pageReturnData();
            data.languageCode = languageCode;
            data.page = new Page();

            using (ApplicationDbContext DB = new ApplicationDbContext())
            {
                try
                {
                    Page page = new Page();
                    data.page = page.getPage(4, languageCode);
                }
                catch (Exception ex)
                {
                    data.message = "Error: " + ex.Message;
                }

            }

            //return Json(data,JsonRequestBehavior.AllowGet);
            return View("terms", data);
        }




        // route schools
        public ActionResult schools(string languageCode)
        {
            pageReturnData data = new pageReturnData();
            data.languageCode = languageCode;
            Page page = new Page();
            data.page = page.getPage(6, languageCode);

            using (ApplicationDbContext DB = new ApplicationDbContext())
            {
                try
                {
                    data.schools = DB.Schools.Include("schoolLocation").ToList();
                    foreach (School s in data.schools)
                    {
                        s.languageCode = languageCode;
                        s.loadContent(DB);
                    }
                }
                catch (Exception ex)
                {
                    data.message = "Error: " + ex.Message;
                }

            }

            //return Json(data,JsonRequestBehavior.AllowGet);
            return View("schools", data);
        }

    }
}