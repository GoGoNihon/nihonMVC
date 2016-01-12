using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoGoNihon_MVC.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity;

namespace GoGoNihon_MVC.Controllers
{
    [Authorize]
    public class APIContentController : Controller
    {

        [Authorize(Roles = "admin")]
        [Route("api/addContent")]
        public ActionResult addContent (Content content)
        {
            ajaxReturnData data = new ajaxReturnData();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
              
                try
                {
                    db.Content.Add(content);
                    db.SaveChanges();
                    data.statusCode = (int)statusCodes.successRun;
                    data.callback = "getContentList";
                    data.message = "Content section '"+content.name+"' added";
                }
                catch (Exception ex)
                {
                    data.statusCode = (int)statusCodes.fail;
                    data.message = "Error: Content not added;  " + ex.Message;
                }

            }


            return Json(data);
        }

        [Authorize(Roles = "admin")]
        [Route("api/editContent/{callback?}")]
        public ActionResult editContent(Content content, string callback)
        {
            ajaxReturnData data = new ajaxReturnData();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {

                try
                {
                    
                    db.Entry(content).State = EntityState.Modified;
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
                    
                    data.message = "Content section '" + content.name + "' updated";
                }
                catch (Exception ex)
                {
                    data.statusCode = (int)statusCodes.fail;
                    data.message = "Error: Content not updated;  " + ex.Message;
                }

            }


            return Json(data);
        }

        [Authorize(Roles = "admin")]
        [Route("api/getContentList/{pageID}")]
        public ActionResult getContentList(int pageID)
        {
            ajaxReturnData data = new ajaxReturnData();

            using (ApplicationDbContext DB = new ApplicationDbContext())
            {
                try
                {
                    data.data = DB.Content.Where(c => c.pageID == pageID).OrderByDescending(c => c.contentID).ToList();
                    //data.data = (from c in db.Content
                    //             where c.pageID == pageID
                    //             select new { name = c.name, pageID = c.pageID, contentCollection = c.contentCollection, contentID = c.contentID }).ToList();

                    //data.data = result;
                    data.statusCode = (int)statusCodes.successRun;
                    data.callback = "fillContentList";
                }
                catch (Exception ex)
                {
                    data.statusCode = (int)statusCodes.fail;
                    data.message = "Error: " + ex.Message;
                }
            }


            return Json(data);
        }



        [Route("api/getContent/{pageID}")]
        public ActionResult getContent(int pageID)
        {
            ajaxReturnData data = new ajaxReturnData();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {

                try
                {

                    var result = (from c in db.Content
                                  where c.pageID == pageID
                                  select new { contentID = c.contentID, name = c.name, pageID = c.pageID, contentCollection = c.contentCollection, languageCollection = c.languageCollection }).ToList()
                                 .Select(c => new Content { contentID = c.contentID, name = c.name, pageID = c.pageID, contentCollection = c.contentCollection, languageCollection = c.languageCollection }).ToList<Content>();

                    List<Content> contentCollection = result;

                    foreach (Content content in contentCollection)
                    {
                        content.loadContent("en");
                    }

                    data.data = contentCollection;
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

        [HttpPost]
        [Route("api/getContentByID/{contentID}/{code=*}")]
        public ActionResult getContentByID(int contentID, string code)
        {
            ajaxReturnData data = new ajaxReturnData();

      
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                try
                {

                    List<Content> contentCollection = (from c in db.Content
                                                 where c.contentID == contentID
                                                 select new {contentID = c.contentID, name = c.name, pageID = c.pageID, contentCollection = c.contentCollection, languageCollection = c.languageCollection }).ToList()
                                 .Select(c => new Content { contentID = c.contentID, name = c.name, pageID = c.pageID, contentCollection = c.contentCollection, languageCollection = c.languageCollection }).ToList<Content>();

                    

                    foreach (Content content in contentCollection)
                    {
                        content.loadContent(code);
                    }

                    data.data = contentCollection;
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

        [HttpPost, ValidateInput(false)]
        [Route("api/addContentBody/{callback?}")]
        public ActionResult addContentBody(ContentBody contentBody, string callback, string name, string code, int contentID = 0)
        {
            ajaxReturnData data = new ajaxReturnData();

            if (!String.IsNullOrWhiteSpace(contentBody.body))
            {

                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    contentBody.lastModified = DateTime.Now;

                    try
                    {

                        if (contentBody.contentBodyID == 0 && !String.IsNullOrEmpty(name) & contentID != 0)
                        {
                            contentBody.contentID = contentID;
                            contentBody.lastModifiedByID = User.Identity.GetUserId();
                            contentBody.lastModified = DateTime.Now;
                            db.ContentBody.Add(contentBody);
                            db.SaveChanges();
                        }
                        else
                        {

                            if (contentBody.contentBodyID == 0)
                            {
                                db.ContentBody.Add(contentBody);
                                db.SaveChanges();
                            }
                            else
                            {


                                ContentBody cb = db.ContentBody.Where(c => c.contentBodyID == contentBody.contentBodyID).FirstOrDefault();
                                cb.body = contentBody.body;
                                cb.lastModified = contentBody.lastModified;
                                cb.lastModifiedByID = contentBody.lastModifiedByID;
                                db.SaveChanges();

                            }
                            
                        }




                        if (string.IsNullOrEmpty(callback))
                        {
                            data.statusCode = (int)statusCodes.success;
                            data.message = "Content updated";
                        }
                        else
                        {
                            data.statusCode = (int)statusCodes.successRun;
                            data.message = "Content updated";
                            data.callback = callback;
                        }


                    }
                    catch (Exception ex)
                    {
                        data.statusCode = (int)statusCodes.fail;
                        data.message = "Error: Content not added;  " + contentBody.contentBodyID + "  ||  " + ex.ToString();
                    }

                }
            }
            else

            {

                data.statusCode = (int)statusCodes.fail;
                data.message = "Error: Cant add nothing... ";
            }

            
            return Json(data);
        }


        [Route("api/getContentBody/{contentBodyID}/{callback?}")]
        public ActionResult getContenetBody (int contentBodyID, string callback)
        {
            ajaxReturnData data = new ajaxReturnData();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                try
                {

                    var result = (from c in db.ContentBody
                                  where c.contentBodyID == contentBodyID
                                  select new {contentBodyID = c.contentBodyID, contentID = c.contentID, body = c.body, code = c.code, language = c.language, lastModified = c.lastModified, lastModifiedByID = c.lastModifiedByID }).ToList();

                    data.data = result;

                    if (string.IsNullOrEmpty(callback))
                    {
                        data.statusCode = (int)statusCodes.success;
                    }
                    else {
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


        [Route("api/getContentBodyByCode/{contentID}/{code}/{callback?}")]
        public ActionResult getContenetBodyByCode(int contentID, string code, string callback)
        {
            ajaxReturnData data = new ajaxReturnData();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                try
                {
                    data.data = (from c in db.ContentBody
                                  where c.contentID == contentID && c.code == code
                                  select new { contentBodyID = c.contentBodyID, contentID = c.contentID, body = c.body, code = c.code, language = c.language, lastModified = c.lastModified, lastModifiedByID = c.lastModifiedByID }).ToList();
                    

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
                catch (Exception ex)
                {
                    data.statusCode = (int)statusCodes.fail;
                    data.message = "Error: " + ex.Message;
                }

            }


            return Json(data);
        }



        [Authorize(Roles = "admin")]
        [Route("api/deleteContent/{contentID}/{callback?}")]
        public ActionResult deleteContent(int contentID, string callback)
        {
            ajaxReturnData data = new ajaxReturnData();

            using (ApplicationDbContext DB = new ApplicationDbContext())
            {
                try
                {

                    Content c = new Content();
                    c.contentID = contentID;

                    DB.Content.Attach(c);
                    DB.Entry(c).State = EntityState.Deleted;
                    DB.SaveChanges();

                    data.message = "Content deleted";

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
                catch (Exception ex)
                {
                    data.statusCode = (int)statusCodes.fail;
                    data.message = "Error: " + ex.Message;
                }

            }


            return Json(data);
        }


    }
}