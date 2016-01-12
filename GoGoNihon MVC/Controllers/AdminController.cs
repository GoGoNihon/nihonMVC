using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoGoNihon_MVC.Models;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using System.Data.Entity;

namespace GoGoNihon_MVC.Models
{

    public struct loginData
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    };

    public struct adminReturnData
    {
        public ICollection<Page> pages;
        public IEnumerable<ApplicationUser> users;
        public IEnumerable<Language> languages;
        public loginData loginData;
        public IEnumerable<IdentityRole> roles;
        public IDictionary<string, string> roleNames;
        
    }
  
}

namespace GoGoNihon_MVC.Controllers
{
    [Authorize(Roles ="admin")]
    public class AdminController : Controller
    {
        
        [Route("admin")]
        public ActionResult Index()
        {
           

            adminReturnData returnData = new adminReturnData();
            
            using (ApplicationDbContext DB = new ApplicationDbContext())
            {
                var data = (from p in DB.Pages
                               select new { name = p.name,description = p.description, pageID = p.pageID }).ToList()
                               .Select(p => new Page { name = p.name, description = p.description, pageID = p.pageID });
                returnData.pages = new List<Page>(data);
                
            }
            

            return View("content", returnData);
        }



        [Route("admin/users")]
        public ActionResult users()
        {

            adminReturnData returnData = new adminReturnData();

            return View("users", returnData);
        }

        [Route("admin/schools")]
        public ActionResult schools()
        {

            adminReturnData returnData = new adminReturnData();
            return View("schools", returnData);
        }

        [Route("admin/courses")]
        public ActionResult courses()
        {

            adminReturnData returnData = new adminReturnData();
            return View("courses", returnData);
        }

        [Route("admin/shortCourses")]
        public ActionResult shortCourses()
        {

            adminReturnData returnData = new adminReturnData();
            return View("shortCourses", returnData);
        }



        [Route("admin/language")]
        public ActionResult languageHome()
        {

            adminReturnData returnData = new adminReturnData();

            //using (ApplicationDbContext DB = new ApplicationDbContext())
            //{
            //    var data = (from p in DB.Pages
            //                select new { name = p.name, controller = p.controller, description = p.description, path = p.path, pageID = p.pageID }).ToList()
            //                    .Select(p => new Page { name = p.name, controller = p.controller, description = p.description, path = p.path, pageID = p.pageID });
            //    returnData.pages = new List<Page>(data);
            //}


            return View("language", returnData);
        }




    }
}