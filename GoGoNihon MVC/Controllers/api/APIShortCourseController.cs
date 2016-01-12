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
    public class APIShortCourseController : Controller
    {
        

        [HttpPost]
        [Route("api/addShortCourse/{callback?}")]
        public ActionResult addShortCourse(string callback, string url, string name)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    if (DB.shortCourses.Where(s => s.url == url).Any())
                    {
                        data.message = "School already be existing; No need to re-exist it";
                        data.statusCode = (int)statusCodes.fail;
                    }
                    else
                    {
                        ShortCourse sc = new ShortCourse();
                        sc.url = url;
                        Faq faq = new Faq();
                        sc.faq = faq;
                        DB.shortCourses.Add(sc);
                        DB.SaveChanges();
                        sc.addContent(DB, "name", name);

                        
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

                data.message = "short course added";
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to add short course; " + ex.Message;
                return Json(data);
            }
        }


        [HttpPost]
        [Route("api/getShortCourses/{callback?}")]
        public ActionResult getShortCourses(string callback)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    data.data = DB.shortCourses.ToList();

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
                data.message = "Failed to get short courses; " + ex.Message;
                return Json(data);
            }
        }


        [HttpPost]
        [Route("api/deleteShortCourse/{id}/{callback?}")]
        public ActionResult deleteShortCourse(int id, string callback)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    ShortCourse sc = DB.shortCourses.Include("plan").Where(s => s.id == id).FirstOrDefault();
                    List<Content> content;
                    List<int> planIDs = new List<int>();

                    foreach (PlanFeature pf in sc.plan)
                    {
                        planIDs.Add(pf.id);
                    }

                    //sc = null;


                    foreach (int planID in planIDs)
                    {
                        PlanFeature p = DB.planFeatures.Where(s => s.id == planID).FirstOrDefault();
                        content = DB.Content.Where(c => c.planFeatureID == p.id).ToList();

                        foreach (Content item in content)
                        {
                            DB.Content.Remove(item);
                        }
                        DB.SaveChanges();
                        DB.planFeatures.Remove(p);
                        DB.SaveChanges();

                    }

                    sc = DB.shortCourses.Include("accommodationGallery").Include("featureGallery").Include("schoolGallery").Include("staffGallery").Include("culturalGalleries").Where(s => s.id == id).FirstOrDefault();
                    ImageGallery gal;
                    

                    if (sc.accommodationGallery != null)
                    {
                        gal = DB.gallerys.Where(g => g.id == sc.accommodationGallery.id).FirstOrDefault();
                        content = DB.Content.Where(c => c.galleryID == gal.id).ToList();

                        foreach (Content item in content)
                        {
                            DB.Content.Remove(item);
                        }
                        DB.SaveChanges();
                        DB.gallerys.Remove(gal);
                    }

                    if (sc.featureGallery != null)
                    {
                        gal = DB.gallerys.Where(g => g.id == sc.featureGallery.id).FirstOrDefault();
                        content = DB.Content.Where(c => c.galleryID == gal.id).ToList();

                        foreach (Content item in content)
                        {
                            DB.Content.Remove(item);
                        }
                        DB.gallerys.Remove(gal);
                    }
                        

                    if (sc.schoolGallery != null)
                    {
                        gal = DB.gallerys.Where(g => g.id == sc.schoolGallery.id).FirstOrDefault();
                        content = DB.Content.Where(c => c.galleryID == gal.id).ToList();

                        foreach (Content item in content)
                        {
                            DB.Content.Remove(item);
                        }
                        DB.gallerys.Remove(gal);
                    }
                        

                    if (sc.staffGallery != null)
                    {
                        gal = DB.gallerys.Where(g => g.id == sc.staffGallery.id).FirstOrDefault();
                        content = DB.Content.Where(c => c.galleryID == gal.id).ToList();

                        foreach (Content item in content)
                        {
                            DB.Content.Remove(item);
                        }
                        DB.gallerys.Remove(gal);
                    }
                        

                    DB.SaveChanges();

                    foreach (ImageGallery g in sc.culturalGalleries)
                    {
                        content = DB.Content.Where(c => c.galleryID == g.id).ToList();

                        foreach (Content item in content)
                        {
                            DB.Content.Remove(item);
                        }
                        DB.gallerys.Remove(g);
                        DB.SaveChanges();
                    }


                    sc = DB.shortCourses.Where(s => s.id == id).FirstOrDefault();
                    DB.shortCourses.Remove(sc);
                    DB.SaveChanges();


                }

                data.message = "short course removed";
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
                data.message = "Failed to remove short courses; " + ex.Message;
                return Json(data);
            }
        }

        [HttpPost]
        [Route("api/getShortCourse/{id}/{callback?}")]
        public ActionResult getShortCourse(int id, string callback)
        {
            ajaxReturnData data = new ajaxReturnData();
            ShortCourse course;

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext()) { 
            

                    course = DB.shortCourses.Where(S => S.id == id).Include(s => s.culturalGalleries).Include("faq.questions.content.contentCollection").FirstOrDefault();
                    
                    course.loadContent(DB);
                   
                data.data = course;

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
                data.message = "Failed to get short course; " + ex.Message;
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost, ValidateInput(false)]
        [Route("api/editShortCourseGeneral/{id}/{callback?}")]
        public ActionResult editShortCourseGeneral(int id, int schoolID, string descriptionHeading, string bookNowLink, string dates, string video, string name, string url, HttpPostedFileWrapper descriptionImage, HttpPostedFileWrapper videoCover, string callback, string languageCode = "en")
        {
            ajaxReturnData data = new ajaxReturnData();
            ShortCourse course = new ShortCourse();
            

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {

                    if (DB.shortCourses.Where(s => s.url == url && s.id != id).Any())
                    {
                        data.message = "short course url is taken";
                        data.statusCode = (int)statusCodes.fail;
                    }
                    else
                    {
                        course = DB.shortCourses.Where(sc => sc.id == id).FirstOrDefault();
                        course.video = video;
                        course.url = url;
                        course.schoolID = schoolID;
                        course.bookNowLink = bookNowLink;

                        if (descriptionImage != null && descriptionImage.ContentLength > 0)
                        {

                            string path = "/content/images/uploads/shortCourses/" + course.id;
                            bool exists = Directory.Exists(Server.MapPath(path));
                            if (!exists)
                            {
                                Directory.CreateDirectory(Server.MapPath(path));
                            }

                            path = path + "/descriptionImage" + Path.GetExtension(descriptionImage.FileName);
                            descriptionImage.SaveAs(Server.MapPath(path));
                            course.descriptionImage = path;
                            DB.Entry(course).Property(l => l.descriptionImage).IsModified = true;
                        }
                        if (videoCover != null && videoCover.ContentLength > 0)
                        {

                            string path = "/content/images/uploads/shortCourses/" + course.id;
                            bool exists = Directory.Exists(Server.MapPath(path));
                            if (!exists)
                            {
                                Directory.CreateDirectory(Server.MapPath(path));
                            }

                            path = path + "/videoCover" + Path.GetExtension(videoCover.FileName);
                            videoCover.SaveAs(Server.MapPath(path));
                            course.videoCover = path;
                            DB.Entry(course).Property(l => l.videoCover).IsModified = true;
                        }

                        course.addContent(DB, "name", name);
                        course.addContent(DB, "dates", dates);
                        course.addContent(DB, "descriptionHeading", descriptionHeading);
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

                data.message = "short course updated";
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to update short course; " + ex.Message;
                return Json(data);
            }
        }


        [HttpPost, ValidateInput(false)]
        [Route("api/addShortCourseFeature/{courseID}/{callback?}")]
        public ActionResult addShortCourseFeature(int courseID, string callback, string text, string heading, HttpPostedFileWrapper galleryImage, string languageCode = "en")
        {
            ajaxReturnData data = new ajaxReturnData();
            ShortCourse course = new ShortCourse();


            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {


                    //course = DB.shortCourses.Where(sc => sc.id == courseID).FirstOrDefault();
                    //ImageGallery gallery;
                    //GalleryImage img = new GalleryImage();

                    //if (DB.gallerys.Where(g => g.id == course.featureGalleryID).Any())
                    //{
                    //    gallery = DB.gallerys.Where(g => g.id == course.featureGalleryID).FirstOrDefault();
                    //}
                    //else
                    //{
                    //    gallery = new ImageGallery();
                    //    DB.gallerys.Add(gallery);
                    //    DB.SaveChanges();
                    //    gallery.addTitle(DB, course.url + " features");
                    //}

                    //course.featureGalleryID = gallery.id;



                    //if (galleryImage != null && galleryImage.ContentLength > 0)
                    //{

                    //    string path = "/content/images/uploads/galleries/" + gallery.id;
                    //    bool exists = Directory.Exists(Server.MapPath(path));
                    //    if (!exists)
                    //    {
                    //        Directory.CreateDirectory(Server.MapPath(path));
                    //    }


                    //    img.galleryID = gallery.id;
                    //    DB.galleryImages.Add(img);
                    //    DB.SaveChanges();

                    //    path = path + "/" + img.id + Path.GetExtension(galleryImage.FileName);
                    //    galleryImage.SaveAs(Server.MapPath(path));
                    //    img.image = path;
                    //    DB.SaveChanges();
                    //}

                    //DB.SaveChanges();
                    //img.addText(DB, text);
                    //img.addTitle(DB, heading);


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

                data.message = "short course feature added";
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to add short course feature; " + ex.Message;
                return Json(data);
            }
        }


        [HttpPost, ValidateInput(false)]
        [Route("api/editShortCourseFeature/{id}/{callback?}")]
        public ActionResult edotShortCourseFeature(int id, string callback, string text, string heading, HttpPostedFileWrapper galleryImage, string languageCode = "en")
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    
                    GalleryImage img = DB.galleryImages.Where(i => i.id == id).FirstOrDefault();
                    

                    if (galleryImage != null && galleryImage.ContentLength > 0)
                    {

                        string path = "/content/images/uploads/galleries/" + img.galleryID;
                        bool exists = Directory.Exists(Server.MapPath(path));
                        if (!exists)
                        {
                            Directory.CreateDirectory(Server.MapPath(path));
                        }


                        path = path + "/" + img.id + Path.GetExtension(galleryImage.FileName);
                        galleryImage.SaveAs(Server.MapPath(path));
                        img.image = path;
                        DB.SaveChanges();
                    }

                    img.addTitle(DB, heading);
                    img.addText(DB, text);


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

                data.message = "short course feature saved";
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to save short course feature; " + ex.Message;
                return Json(data);
            }
        }


        [HttpPost]
        [Route("api/deleteShortCourseFeature/{id}/{callback?}")]
        public ActionResult deleteShortCourseFeature(int id, string callback)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {

                    GalleryImage img = DB.galleryImages.Where(i => i.id == id).FirstOrDefault();
                    DB.galleryImages.Remove(img);
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

                data.message = "short course feature is gone for all eternity";
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "failed to remove short course feature; " + ex.Message;
                return Json(data);
            }
        }

        [HttpPost]
        [Route("api/addShortCourseCulturalGallery/{courseID}/{callback?}")]
        public ActionResult addShortCourseCulturalGallery(int courseID, string callback, string name)
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

                    ShortCourse sc = DB.shortCourses.Where(s => s.id == courseID).Include("culturalGalleries").FirstOrDefault();
                    sc.culturalGalleries.Add(gal);
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

                data.message = "cultural activity gallery added";
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "failed to add short course cultrual gallery; " + ex.Message;
                return Json(data);
            }
        }


        [HttpPost]
        [Route("api/getCulturalGalleries/{courseID}/{callback?}")]
        public ActionResult getCulturalGalleries(int courseID, string callback)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    ShortCourse sc = DB.shortCourses.Where(s => s.id == courseID).Include("culturalGalleries").FirstOrDefault();
                    foreach (ImageGallery gal in sc.culturalGalleries)
                    {
                        gal.loadContent(DB);
                    }
                    data.data = sc.culturalGalleries.ToList();
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
                data.message = "failed to get short course cultrual galleries; " + ex.Message;
                return Json(data);
            }
        }


        [HttpPost]
        [Route("api/addPlanFeature/{courseID}/{callback?}")]
        public ActionResult addPlanFeature(string name, string callback, int courseID, bool gold, bool silver, bool bronze, string languageCode = "en" )
        {
            ajaxReturnData data = new ajaxReturnData();
            
            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    ShortCourse sc = DB.shortCourses.Where(s => s.id == courseID).FirstOrDefault();
                    PlanFeature p = new PlanFeature();
                    sc.plan.Add(p);
                    DB.SaveChanges();
                    p.addTitle(DB, name);
                    p.bronze = bronze;
                    p.silver = silver;
                    p.gold = gold;
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

                data.message = "plan feature added";
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to add plan feature; " + ex.Message;
                return Json(data);
            }
        }

        [HttpPost]
        [Route("api/getPlan/{courseID}/{callback?}")]
        public ActionResult getPlan(string callback,int courseID, string languageCode = "en")
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    ShortCourse sc = DB.shortCourses.Where(s => s.id == courseID).Include("plan").FirstOrDefault();
                    foreach (PlanFeature p in sc.plan)
                    {
                        p.loadContent(DB);
                    }

                    data.data = sc.plan;
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
                data.message = "Failed to get plan; " + ex.Message;
                return Json(data);
            }
        }


        [HttpPost]
        [Route("api/deletePlanFeature/{id}/{callback?}")]
        public ActionResult deletePlanFeature(int id, string callback)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    PlanFeature p = DB.planFeatures.Where(s => s.id == id).FirstOrDefault();

                    List<Content> content = DB.Content.Where(c => c.planFeatureID == p.id).ToList();

                    foreach (Content item in content)
                    {
                        DB.Content.Remove(item);
                    }
                    DB.SaveChanges();
                    DB.planFeatures.Remove(p);
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

                data.message = "plan feature deleted";
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to add plan feature; " + ex.Message;
                return Json(data);
            }
        }


        [HttpPost]
        [Route("api/editPlanFeature/{id}/{callback?}")]
        public ActionResult editPlanFeature(string callback, int id, bool gold, bool silver, bool bronze)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    PlanFeature p = DB.planFeatures.Where(s => s.id == id).FirstOrDefault();
                    p.bronze = bronze;
                    p.silver = silver;
                    p.gold = gold;
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

                data.message = "plan feature updated";
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to update plan feature; " + ex.Message;
                return Json(data);
            }
        }

        [HttpPost]
        [Route("api/getShortCourseGalleries/{courseID}/{callback?}")]
        public ActionResult getShortCourseGalleries(int courseID, string callback)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    ShortCourse sc = DB.shortCourses.Where(s => s.id == courseID)
                        .Include("featureGallery")
                        .Include("schoolGallery")
                        .Include("accommodationGallery")
                        .Include("staffGallery")
                        .FirstOrDefault();

                    bool featureGalleryNew = false;
                    if (sc.featureGallery == null)
                    {
                        ImageGallery gal = new ImageGallery();
                        sc.featureGallery = gal;
                        featureGalleryNew = true;
                    }

                    bool schoolGalleryNew = false;
                    if (sc.schoolGallery == null)
                    {
                        ImageGallery gal = new ImageGallery();
                        sc.schoolGallery = gal;
                        schoolGalleryNew = true;
                    }

                    bool accommodationGalleryNew = false;
                    if (sc.accommodationGallery == null)
                    {
                        ImageGallery gal = new ImageGallery();
                        sc.accommodationGallery = gal;
                        accommodationGalleryNew = true;
                    }
                    bool staffGalleryNew = false;
                    if (sc.staffGallery == null)
                    {
                        ImageGallery gal = new ImageGallery();
                        sc.staffGallery = gal;
                        staffGalleryNew = true;
                    }

                    DB.SaveChanges();

                    sc = DB.shortCourses.Where(s => s.id == courseID)
                        .Include("featureGallery")
                        .Include("schoolGallery")
                        .Include("accommodationGallery")
                        .Include("staffGallery")
                        .FirstOrDefault();
                    

                    if(featureGalleryNew)
                    {
                        sc.featureGallery.addTitle(DB, "short course feature gallery");
                    }
                    if (staffGalleryNew)
                    {
                        sc.staffGallery.addTitle(DB, "short course staff gallery");
                    }
                    if (accommodationGalleryNew)
                    {
                        sc.accommodationGallery.addTitle(DB, "short course accommodation gallery");
                    }
                    if (schoolGalleryNew)
                    {
                        sc.schoolGallery.addTitle(DB, "schort course school gallery");
                    }
                    

                    sc.featureGallery.loadContent(DB);
                    sc.staffGallery.loadContent(DB);
                    sc.accommodationGallery.loadContent(DB);
                    sc.schoolGallery.loadContent(DB);

                    List<ImageGallery> galleries = new List<ImageGallery>();
                    galleries.Add(sc.featureGallery);
                    galleries.Add(sc.schoolGallery);
                    galleries.Add(sc.accommodationGallery);
                    galleries.Add(sc.staffGallery);

                    data.data = galleries;
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
                data.message = "Failed to get general short course galleries; " + ex.Message;
                return Json(data);
            }
        }

        [ValidateInput(false)]
        [HttpPost]
        [Route("api/addShortCourseQuestion/{courseID}/{callback?}")]
        public ActionResult addShortCourseQuestion(string callback, int courseID, string question, string answer)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    ShortCourse sc = DB.shortCourses.Include("faq.questions").Where(s => s.id == courseID).FirstOrDefault();
                    FaqQuestion fq = new FaqQuestion();
                    fq.udate(DB, question, answer);
                    sc.faq.questions.Add(fq);
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

                data.message = "question added";
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to add question; " + ex.Message;
                return Json(data);
            }
        }

        


    }
}