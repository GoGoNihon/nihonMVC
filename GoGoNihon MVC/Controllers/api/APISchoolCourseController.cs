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
    public class APISchoolCourseController : Controller
    {
        [HttpPost, ValidateInput(false)]
        [Route("api/addCourse/{schoolID}/{callback?}")]
        public ActionResult addCourse(string callback, int schoolID, int type, string name, string courseIntroduction,  HttpPostedFileWrapper introductionImage)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {


                    SchoolCourse sc = new SchoolCourse();
                    sc.schoolID = schoolID;
                    sc.type = type;
                    DB.schoolCourses.Add(sc);
                    sc.features = new CourseFeatures();
                    DB.SaveChanges();
                    sc.addContent(DB, "name", name);
                    sc.addContent(DB, "courseIntroduction", courseIntroduction);

                    if (introductionImage != null && introductionImage.ContentLength > 0)
                    {

                        string path = "/content/images/uploads/courses/" + sc.id;
                        bool exists = Directory.Exists(Server.MapPath(path));
                        if (!exists)
                        {
                            Directory.CreateDirectory(Server.MapPath(path));
                        }

                        path = path + "/introductionImage" + Path.GetExtension(introductionImage.FileName);
                        introductionImage.SaveAs(Server.MapPath(path));
                        sc.introductionImage = path;
                        DB.Entry(sc).Property(l => l.introductionImage).IsModified = true;
                        DB.SaveChanges();
                    }




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

                data.message = "course added";
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to add course; " + ex.Message;
                return Json(data);
            }
        }


        [HttpPost]
        [Route("api/getCourses/{schoolID}/{callback?}")]
        public ActionResult getCourses(string callback, int schoolID)
        {
            ajaxReturnData data = new ajaxReturnData();
            List<SchoolCourse> courses = new List<SchoolCourse>();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {


                   courses = (from sc in DB.schoolCourses
                     where sc.schoolID == schoolID
                     select new {
                         sc.demographyImage,
                         sc.hoursPerWeek,
                         sc.id,
                         sc.introductionImage,
                         sc.languageCode,
                         sc.schoolID,
                         sc.studentsPerClass,
                         sc.totalStudents}).ToList()
                               .Select(sc => new SchoolCourse {
                                   demographyImage= sc.demographyImage,
                                   hoursPerWeek= sc.hoursPerWeek,
                                   id=sc.id,
                                   introductionImage=sc.introductionImage,
                                   languageCode=sc.languageCode,
                                   schoolID=sc.schoolID,
                                   studentsPerClass=sc.studentsPerClass,
                                   totalStudents=sc.totalStudents }).ToList<SchoolCourse>();

                    foreach (SchoolCourse sc in courses)
                    {
                        sc.loadContent(DB);
                    }
                }

                data.data = courses;


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
                data.message = "Failed to retrieve courses; " + ex.Message;
                return Json(data);
            }
        }


        [HttpPost]
        [Route("api/getCourse/{courseID}/{languageCode?}/{callback?}")]
        public ActionResult getCourse(string callback, int courseID, string languageCode="en")
        {
            ajaxReturnData data = new ajaxReturnData();
            SchoolCourse course = new SchoolCourse();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    course = DB.schoolCourses.Where(c => c.id == courseID).FirstOrDefault();
                    course.loadContent(DB);
                }

                data.data = course;


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
                data.message = "Failed to retrieve course; " + ex.Message;
                return Json(data);
            }
        }


        [HttpPost, ValidateInput(false)]
        [Route("api/editCourseGeneral/{courseID}/{languageCode?}/{callback?}")]
        public ActionResult editCourseGeneral(string name, int type, string editCourseIntroduction, HttpPostedFileWrapper introductionImage, string callback, int courseID, string languageCode = "en" )
        {
            ajaxReturnData data = new ajaxReturnData();
            SchoolCourse course = new SchoolCourse();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {

                    course = DB.schoolCourses.Where(sc => sc.id == courseID).FirstOrDefault();
                    course.type = type;
                    DB.SaveChanges();

                    if (introductionImage != null && introductionImage.ContentLength > 0)
                    {

                        string path = "/content/images/uploads/courses/" + course.id;
                        bool exists = Directory.Exists(Server.MapPath(path));
                        if (!exists)
                        {
                            Directory.CreateDirectory(Server.MapPath(path));
                        }

                        path = path + "/introductionImage" + Path.GetExtension(introductionImage.FileName);
                        introductionImage.SaveAs(Server.MapPath(path));
                        course.introductionImage = path;
                        DB.Entry(course).Property(l => l.introductionImage).IsModified = true;
                    }
                    
                    course.addContent(DB, "name",name);
                    course.addContent(DB, "courseIntroduction", editCourseIntroduction);
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

                data.message = "course updated";
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to retrieve course; " + ex.Message;
                return Json(data);
            }
        }

        [HttpPost]
        [Route("api/addCourseLength/{courseID}/{callback?}")]
        public ActionResult addCourseLength(string callback, int courseID, CourseLength length)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {

                    length.courseID = courseID;
                    DB.courseLengths.Add(length);
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

                data.message = "course length added";
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to add course length; " + ex.Message;
                return Json(data);
            }
        }


        [HttpPost]
        [Route("api/getCourseLengths/{courseID}/{callback?}")]
        public ActionResult getCourseLengths(string callback, int courseID)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {

                    data.data = (from cl in DB.courseLengths
                                 where cl.courseID == courseID
                                 select new { cl.courseID,cl.@from,cl.id,cl.to, cl.withVisa  }).ToList();
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
                data.message = "Failed to add course length; " + ex.Message;
                return Json(data);
            }
        }



        [HttpPost]
        [Route("api/deleteLength/{lengthID}/{callback?}")]
        public ActionResult deleteLength(string callback, int lengthID)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {

                    CourseLength cl = DB.courseLengths.Where(l => l.id == lengthID).FirstOrDefault();
                    DB.courseLengths.Remove(cl);
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

                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to remove course length; " + ex.Message;
                return Json(data);
            }
        }


        [HttpPost, ValidateInput(false)]
        [Route("api/editSchedule/{courseID}/{languageCode?}/{callback?}")]
        public ActionResult editSchedule(int hoursPerWeek, string scheduleText, string callback, int courseID, string languageCode = "en")
        {
            ajaxReturnData data = new ajaxReturnData();
            SchoolCourse course = new SchoolCourse();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {

                    course = DB.schoolCourses.Where(sc => sc.id == courseID).FirstOrDefault();
                    course.hoursPerWeek = hoursPerWeek;
                    course.addContent(DB, "scheduleText", scheduleText);
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

                data.message = "schedule updated";
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to update sschedule; " + ex.Message;
                return Json(data);
            }
        }


        [HttpPost, ValidateInput(false)]
        [Route("api/addTermStep/{courseID}/{languageCode?}/{callback?}")]
        public ActionResult addTermStep(string addTermBody, string callback, int courseID, string languageCode = "en")
        {
            ajaxReturnData data = new ajaxReturnData();
            TermBreakdownStep termStep = new TermBreakdownStep();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {

                    SchoolCourse course = DB.schoolCourses.Include("termSteps").Where(c => c.id == courseID).FirstOrDefault();


                    int index = course.termSteps.Count;
                    index++;
                    termStep.index = index;
                    course.termSteps.Add(termStep);
                    DB.SaveChanges();
                    termStep.addContent(DB, "termBreakdownText", addTermBody);

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

                data.message = "term breakdown step added";
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to add term breakdown step; " + ex.Message;
                return Json(data);
            }
        }


        [HttpPost, ValidateInput(false)]
        [Route("api/editTermStep/{stepID}/{languageCode?}/{callback?}")]
        public ActionResult editTermStep(string editTermBreakdownBody, string callback, int stepID, string languageCode = "en")
        {
            ajaxReturnData data = new ajaxReturnData();
            

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    TermBreakdownStep step = DB.termSteps.Where(s => s.id == stepID).FirstOrDefault();
                    DB.SaveChanges();
                    step.addContent(DB, "termBreakdownText", editTermBreakdownBody);
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

                data.message = "term breakdown step edited";
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to edit term breakdown step; " + ex.Message;
                return Json(data);
            }
        }

        [HttpPost, ValidateInput(false)]
        [Route("api/getTermSteps/{courseID}/{languageCode?}/{callback?}")]
        public ActionResult getTermSteps(string callback, int courseID, string languageCode = "en")
        {
            ajaxReturnData data = new ajaxReturnData();


            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {

                    SchoolCourse course = DB.schoolCourses.Include("termSteps.stepContent.contentCollection").Where(c => c.id == courseID).FirstOrDefault();

                    List<TermBreakdownStep> steps = course.termSteps.OrderBy(l => l.index).ToList();

                    foreach (TermBreakdownStep step in steps)
                    {
                        step.loadContent(DB);
                    }

                    data.data = steps;
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
                data.message = "Failed to get term breakdown steps; " + ex.Message;
                return Json(data);
            }
        }

        [HttpPost]
        [Route("api/deleteStep/{stepID}/{courseID}/{callback?}")]
        public ActionResult deleteStep(string callback, int courseID, int stepID)
        {
            ajaxReturnData data = new ajaxReturnData();


            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {

                    SchoolCourse course = DB.schoolCourses.Include("termSteps").Where(c => c.id == courseID).FirstOrDefault();

                    TermBreakdownStep ts = course.termSteps.Where(t => t.id == stepID).FirstOrDefault();
                    int index = ts.index;
                    course.termSteps.Remove(ts);
                    DB.SaveChanges();

                    //List<TermBreakdownStep> steps = .ToList();

                    foreach (TermBreakdownStep step in course.termSteps)
                    {
                        if(step.index > index)
                        {
                            step.index = (step.index - 1);
                        }
                    }

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

                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to get term breakdown steps; " + ex.Message;
                return Json(data);
            }
        }


        [HttpPost]
        [Route("api/getStep/{stepID}/{callback?}")]
        public ActionResult getStep(string callback, int stepID)
        {
            ajaxReturnData data = new ajaxReturnData();


            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {

                    TermBreakdownStep step = DB.termSteps.Include("stepContent.contentCollection").Where(t => t.id == stepID).FirstOrDefault();
                    step.loadContent(DB);
                    data.data = step;
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
                data.message = "Failed to get term breakdown step; " + ex.Message;
                return Json(data);
            }
        }


       

        [HttpPost]
        [Route("api/updateStepsOrder/{callback?}")]
        public ActionResult updateStepOrder(string steps, string callback)
        {
            ajaxReturnData data = new ajaxReturnData();


            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    JArray j = JArray.Parse(steps);
                    foreach (var item in j)
                    {
                        int id = Convert.ToInt16(item["id"]);
                        TermBreakdownStep t = DB.termSteps.Where(ts => ts.id == id).FirstOrDefault();
                        t.index = Convert.ToInt16(item["index"]);
                        DB.SaveChanges();

                    }

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
                data.message = "step order updated";
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to update term breakdown step order; " + ex.Message;
                return Json(data);
            }
        }


        [HttpPost, ValidateInput(false)]
        [Route("api/editTermStarts/{id}/{callback?}")]
        public ActionResult editTermStarts(int id, string termStarts, string callback)
        {
            ajaxReturnData data = new ajaxReturnData();


            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    SchoolCourse sc = DB.schoolCourses.Where(s => s.id == id).FirstOrDefault();
                    sc.addContent(DB,"termStarts", termStarts);
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
                data.message = "term stats updated";
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to update term starts; " + ex.Message;
                return Json(data);
            }
        }

        [HttpPost, ValidateInput(false)]
        [Route("api/editCourseFeatures/{id}/{callback?}")]
        public ActionResult editCourseFeatures(CourseFeatures features, int id, string callback)
        {
            ajaxReturnData data = new ajaxReturnData();


            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    SchoolCourse sc = DB.schoolCourses.Where(s => s.id == id).FirstOrDefault();
                    sc.features = features;
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
                data.message = "features updated";
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to update features; " + ex.Message;
                return Json(data);
            }
        }


        [HttpPost, ValidateInput(false)]
        [Route("api/editCosts/{id}/{callback?}")]
        public ActionResult editCosts(string costsText, int id, string callback)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    SchoolCourse sc = DB.schoolCourses.Where(s => s.id == id).FirstOrDefault();
                    sc.addContent(DB, "costsText", costsText);
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
                data.message = "costs updated";
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to update costs; " + ex.Message;
                return Json(data);
            }
        }


        [HttpPost]
        [Route("api/addDemographyImage/{courseID}/{callback?}")]
        public ActionResult addDemography(int courseID, string callback, HttpPostedFileWrapper demographyImage)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    SchoolCourse course = DB.schoolCourses.Where(c => c.id == courseID).FirstOrDefault();


                    if (demographyImage != null && demographyImage.ContentLength > 0)
                    {

                        string path = "/content/images/uploads/courses/" + course.id;
                        bool exists = Directory.Exists(Server.MapPath(path));
                        if (!exists)
                        {
                            Directory.CreateDirectory(Server.MapPath(path));
                        }

                        path = path + "/demographyImage" + Path.GetExtension(demographyImage.FileName);
                        demographyImage.SaveAs(Server.MapPath(path));
                        course.demographyImage = path;
                        DB.SaveChanges();
                    }
                    

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
                data.message = "Failed to add demo image; " + ex.Message;
                return Json(data);
            }
        }


        [HttpPost]
        [Route("api/addDemography/{courseID}/{callback?}")]
        public ActionResult addDemography(CourseDemography demo, string name, int courseID, string callback)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    demo.schoolCourseID = courseID;
                    DB.demographies.Add(demo);
                    DB.SaveChanges();
                    demo.addContent(DB, "name", name);
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
                data.message = "Failed to add demo; " + ex.Message;
                return Json(data);
            }
        }


        [HttpPost]
        [Route("api/getDemographics/{courseID}/{callback?}")]
        public ActionResult getDemographics(string callback, int courseID)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {

                    List<CourseDemography> demos = (from d in DB.demographies
                                              where d.schoolCourseID == courseID
                                              select new
                                              {
                                                  d.content,
                                                  d.id,
                                                  d.languageCode,
                                                  d.percent,
                                                  d.schoolCourseID
                                              }).ToList()
                     .Select(d => new CourseDemography
                     {
                        content = d.content,
                        id = d.id,
                        languageCode = d.languageCode,
                        percent = d.percent,
                        schoolCourseID = d.schoolCourseID
                     }).ToList<CourseDemography>();


                    foreach (var demo in demos)
                    {
                        demo.loadContent(DB);
                    }
                    
                    data.data = demos;
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
                data.message = "Failed to load demos; " + ex.Message;
                return Json(data);
            }
        }

        [HttpPost]
        [Route("api/deleteDemography/{id}/{callback?}")]
        public ActionResult deleteDemography(string callback, int id)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {

                    CourseDemography demo = DB.demographies.Where(d => d.id == id).FirstOrDefault();
                    DB.demographies.Remove(demo);
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

                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to remove demo; " + ex.Message;
                return Json(data);
            }
        }


        [HttpPost]
        [Route("api/deleteCourse/{id}/{callback?}")]
        public ActionResult deleteCourse(string callback, int id)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {

                    SchoolCourse sc = DB.schoolCourses.Where(c => c.id == id).FirstOrDefault();
                    DB.schoolCourses.Remove(sc);
                    DB.SaveChanges();
                }

                data.message = "course deleted";
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
                data.message = "Failed to remove course; " + ex.Message;
                return Json(data);
            }
        }


    }
}