using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoGoNihon_MVC.Models
{




    public class Functions
    {



        public static List<SchoolCourse> getSchoolCourses(int schoolID, string languageCode, bool withAdminControls = false)
        {
            List<SchoolCourse> courses;
            using (ApplicationDbContext DB = new ApplicationDbContext())
            {
                courses = DB.schoolCourses
                    .Include("courseContent.contentCollection")
                    .Include("courseDemographies.content.contentCollection")
                    .Include("courseLengths")
                    .Include("termSteps.stepContent.contentCollection")
                    .Where(c => c.schoolID == schoolID).ToList();

                foreach(SchoolCourse course in courses)
                {
                    course.languageCode = languageCode;
                    course.loadContent(DB, withAdminControls);

                    foreach (TermBreakdownStep step in course.termSteps)
                    {
                        step.loadContent(DB);
                    }
                    foreach (CourseDemography demo in course.courseDemographies)
                    {
                        demo.loadContent(DB);
                    }
                    course.courseDemographies = course.courseDemographies.OrderByDescending(c => c.percent).ToList();
                }
                
            }
                return courses;
        }



        //helper crap shoved here



        //////////////////  get a user  /////////////////////////////////////////
        //ApplicationUser user = new ApplicationUser();

        //var store = new UserStore<ApplicationUser>(DB);
        //UserManager<ApplicationUser> um = new UserManager<ApplicationUser>(store);
        //user = um.FindByEmail("some@dude.com");



        ///////////  add role to user  //////////////////////////////////////////////////////////
        //ApplicationUser user = new ApplicationUser();

        //var store = new UserStore<ApplicationUser>(DB);
        //UserManager<ApplicationUser> um = new UserManager<ApplicationUser>(store);
        //user = um.FindByEmail("nathan@gogoworld.com");
        //await store.AddToRoleAsync(user, "admin");
        //await store.UpdateAsync(user);



        /////////  update a password  /////////////////////
        //ApplicationUser user = new ApplicationUser();

        //var store = new UserStore<ApplicationUser>(DB);
        //UserManager<ApplicationUser> um = new UserManager<ApplicationUser>(store);
        //user = um.FindByEmail("nathan@gogoworld.com");
        //        PasswordHasher ph = new PasswordHasher();
        //await store.SetPasswordHashAsync(user, ph.HashPassword("testpass888"));

        //        await store.UpdateAsync(user);




        ////////////////  how to add a role  //////////////////////////
        //IdentityRole role = new IdentityRole();
        //role.Name = "test role";
        //DB.Roles.Add(role);
        //DB.SaveChanges();



    }
}