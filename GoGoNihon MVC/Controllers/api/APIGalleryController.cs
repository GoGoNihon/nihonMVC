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
    public class APIGalleryController : Controller
    {
        [HttpPost]
        [Route("api/getGallery/{id}/{callback?}")]
        public ActionResult getGallery(int id, string callback)
        {
            ajaxReturnData data = new ajaxReturnData();
            List<GalleryImage> images = new List<GalleryImage>();
            ImageGallery gal;
            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {

                    gal = DB.gallerys.Where(g => g.id == id).FirstOrDefault();
                    gal.loadContent(DB);

                    gal.galleryImages = DB.galleryImages.Where(img => id == img.galleryID).ToList();

                    foreach (GalleryImage img in gal.galleryImages)
                    {
                        img.loadContent(DB);
                    }
                }

                data.data = gal;


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
                data.message = "Failed to get gallery; " + ex.Message;
                return Json(data);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/deleteGallery/{id}/{callback?}")]
        public ActionResult deleteGallery(int id, string callback)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    
                    ImageGallery gal = DB.gallerys.Where(g => g.id == id).FirstOrDefault();
                    DB.gallerys.Remove(gal);
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
        [HttpPost, ValidateInput(false)]
        [Route("api/addGalleryImage/{galleryID}/{callback?}")]
        public ActionResult addGalleryImage(int galleryID, string callback, HttpPostedFileWrapper image, string title, string text, string tag)
        {
            ajaxReturnData data = new ajaxReturnData();

            using (ApplicationDbContext DB = new ApplicationDbContext())
            {
                ImageGallery gal = DB.gallerys.Where(g => g.id == galleryID).FirstOrDefault();

                if (gal.addImage(galleryID, title, text, image, tag))
                {

                    data.message = "image added";

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
                else
                {
                    data.statusCode = (int)statusCodes.fail;
                    data.message = "Failed to add gallery image";
                    return Json(data);
                }
            }
        }


        [HttpPost]
        [Route("api/editGallery/{galleryID}/{callback?}")]
        public ActionResult editGallery(int galleryID, string callback, string name)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    
                    ImageGallery gal = DB.gallerys.Where(g => g.id == galleryID).FirstOrDefault();
                    gal.addTitle(DB, name);
                    DB.SaveChanges();

                    data.message = "gallery name updated";

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
                data.message = "Failed to update gallery; " + ex.Message;
                return Json(data);
            }
        }

        [Authorize]
        [HttpPost, ValidateInput(false)]
        [Route("api/editGalleryImage/{id}/{callback?}")]
        public ActionResult editGalleryImage(int id, string callback, string title, string text, HttpPostedFileWrapper galleryImage, string tag)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    GalleryImage image = DB.galleryImages.Where(g => g.id == id).FirstOrDefault();
                    string asfs= image.tag;
                    image.udate(DB, title, text, galleryImage, tag);
                    DB.SaveChanges();

                    data.message = "image updated";

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
                data.message = "Failed to update image; " + ex.Message;
                return Json(data);
            }
        }


        [Authorize]
        [HttpPost]
        [Route("api/deleteGalleryImage/{id}/{callback?}")]
        public ActionResult deleteGalleryImage(int id, string callback)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {

                    GalleryImage img = DB.galleryImages.Where(g => g.id == id).FirstOrDefault();
                    DB.galleryImages.Remove(img);
                    DB.SaveChanges();

                    data.message = "image deleted";

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
                data.message = "Failed to delete image; " + ex.Message;
                return Json(data);
            }
        }

        


    }
}