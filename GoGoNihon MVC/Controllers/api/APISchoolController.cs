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

namespace GoGoNihon_MVC.Controllers
{
    public class APISchoolController : Controller
    {

        [Authorize(Roles = "admin")]
        [HttpPost, ValidateInput(false)]
        [Route("api/createSchool/{callback?}")]
        public ActionResult createSchool(string name, int type, string url, string callback)
        {
            ajaxReturnData data = new ajaxReturnData();

            try {

                using (ApplicationDbContext DB = new ApplicationDbContext())
                {

                    if (DB.Schools.Where(s => s.url == url).Any())
                    {
                        data.message = "School already be existing; No need to re-exist it";
                        data.statusCode = (int)statusCodes.fail;
                    }
                    else
                    {
                        School school = new School();
                        school.features = new SchoolFeatures();
                        school.content = new List<Content>();
                        school.type = type;
                        school.url = url;
                        
                        DB.Schools.Add(school);
                        DB.SaveChanges();

                        school.addContent(DB, "name", name);

                        data.message = "School "+name+" added.";

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
                }
                

            }
            catch(Exception ex)
            {
                data.message = "Failed to create school; " + ex.Message;
                data.statusCode = (int)statusCodes.fail;
            }

            
            return Json(data);
        }

        [HttpPost, Authorize]
        [Route("api/deleteSchool/{schoolID}/{callback?}")]
        public ActionResult deleteSchool(int schoolID, string callback)
        {
            ajaxReturnData data = new ajaxReturnData();

            using (ApplicationDbContext DB = new ApplicationDbContext())
            {
                try
                {
                    School doomedSchool = new School();
                    doomedSchool.id = schoolID;
                    doomedSchool.features = new SchoolFeatures();
                    DB.Schools.Attach(doomedSchool);
                    DB.Schools.Remove(doomedSchool);

                    DB.SaveChanges();



                    if (string.IsNullOrEmpty(callback))
                    {
                        data.statusCode = (int)statusCodes.success;
                    }
                    else
                    {
                        data.statusCode = (int)statusCodes.successRun;
                        data.callback = callback;
                    }

                    data.message = "school removed";
                    return Json(data);
                }
                catch (Exception ex)
                {
                    data.statusCode = (int)statusCodes.fail;
                    data.message = "Error: " + ex.Message;
                    return Json(data);
                }

            }
        }


        [Route("api/getSchoolList/{callback?}")]
        public ActionResult getSchoolList(string callback)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {

                    data.data = (from s in DB.Schools
                                 select new { name = s.url, id = s.id }).ToList();

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
            catch(Exception ex)
            {

                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to retrieve school list; " + ex.Message;
                return Json(data);
            }
            
        }

        [HttpPost]
        [Route("api/getSchool/{schoolID?}")]
        public ActionResult getSchool(int schoolID)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {

                    School school = DB.Schools.Include("schoolStations").Include("schoolLocation").Where(s => s.id == schoolID).FirstOrDefault();
             
                    school.loadContent(DB, false);
                    data.data = school;
                }

             
                data.statusCode = (int)statusCodes.success;
                return Json(data);

            }
            catch (Exception ex)
            {

                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to retrieve school; " + ex.Message;
                return Json(data);
            }

        }

        [HttpPost, ValidateInput(false)]
        [Route("api/editSchoolGeneral/{callback?}")]
        public ActionResult editSchoolGeneral(School school, string name, string callback, HttpPostedFileWrapper videoCover)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    

                    if (DB.Schools.Where(s => s.url == school.url && s.id != school.id).Any())
                    {
                        data.message = "url is already assigned to another school";
                        data.statusCode = (int)statusCodes.fail;
                    }
                    else{

                        school.addContent(DB, "name", name);

                        //school.id = schoolID;
                        school.features = new SchoolFeatures();


                        //attach school to database then only update fields that arent multilanguage
                        DB.Schools.Attach(school);
                        DB.Entry(school).State = EntityState.Unchanged;
                        DB.Entry(school).Property(s => s.type).IsModified = true;
                        DB.Entry(school).Property(s => s.url).IsModified = true;
                        DB.Entry(school).Property(s => s.previewVideo).IsModified = true;
                        DB.Entry(school).Property(s => s.video).IsModified = true;
                        DB.Entry(school).Property(s => s.address).IsModified = true;
                        DB.Entry(school).Property(s => s.googleMap).IsModified = true;

                        school.loadContent(DB);

                        



                        if (videoCover != null && videoCover.ContentLength > 0)
                        {

                            string path = "/content/images/uploads/schools/" + school.id;
                            bool exists = Directory.Exists(Server.MapPath(path));
                            if (!exists)
                            {
                                Directory.CreateDirectory(Server.MapPath(path));
                            }

                            path = path + "/cover" + Path.GetExtension(videoCover.FileName);
                            videoCover.SaveAs(Server.MapPath(path));
                            school.videoCover = path;
                            DB.Entry(school).Property(s => s.videoCover).IsModified = true;
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

                        data.message = "school '" + school.url + "' updated";
                    }
                    
                }

                return Json(data);

            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to update school; " + ex.Message;
                return Json(data);
            }
        }

        [HttpPost, ValidateInput(false)]
        [Route("api/editSchoolIntroIntensity/{schoolID}/{callback?}")]
        public ActionResult editSchoolIntroIntensity(string introduction, int schoolID, int intensity, string callback, HttpPostedFileWrapper introductionImage, HttpPostedFileWrapper intensityImage)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {

                    School school = DB.Schools.Where(s => s.id == schoolID).FirstOrDefault();
                    school.addContent(DB, "introduction", introduction);
                    DB.SaveChanges();

                    school.intensity = intensity;

                    if (introductionImage != null && introductionImage.ContentLength > 0)
                    {

                        string path = "/content/images/uploads/schools/" + schoolID;
                        bool exists = Directory.Exists(Server.MapPath(path));
                        if (!exists)
                        {
                            Directory.CreateDirectory(Server.MapPath(path));
                        }

                        path = path + "/introductionImage" + Path.GetExtension(introductionImage.FileName);
                        introductionImage.SaveAs(Server.MapPath(path));
                        school.introductionImage = path;
                    }


                    if (intensityImage != null && intensityImage.ContentLength > 0)
                    {

                        string path = "/content/images/uploads/schools/" + schoolID;
                        bool exists = Directory.Exists(Server.MapPath(path));
                        if (!exists)
                        {
                            Directory.CreateDirectory(Server.MapPath(path));
                        }

                        path = path + "/intensityImage" + Path.GetExtension(intensityImage.FileName);
                        intensityImage.SaveAs(Server.MapPath(path));
                        school.intensityImage = path;
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

                data.message = "school updated";
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to update school; " + ex.Message;
                return Json(data);
            }
        }



        [HttpPost, ValidateInput(false)]
        [Route("api/editSchoolFeatures/{schoolID}/{callback?}")]
        public ActionResult editSchoolFeatures(int schoolID, School school, string callback, string uniqueFeatureText, HttpPostedFileWrapper featuresImage, string uniqueFeature = "")
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    school.id = schoolID;

                    if (!String.IsNullOrWhiteSpace(uniqueFeature))
                    {
                        school.addContent(DB, "USP", uniqueFeature);
                        school.addContent(DB, "uniqueFeatureText", uniqueFeatureText);
                        DB.SaveChanges();
                    }

                    

                    //features is a complex type, meaning its adding as table columns to schools and can not be null on an update
                    if (school.features == null)
                    {
                        school.features = new SchoolFeatures();
                    }


                    DB.Schools.Attach(school);
                    DB.Entry(school).State = EntityState.Unchanged;

                    if(school.features != null)
                    {
                        DB.Entry(school).Property(s => s.features.studentCafe).IsModified = true;
                        DB.Entry(school).Property(s => s.features.acceptsBeginners).IsModified = true;
                        DB.Entry(school).Property(s => s.features.wifiInCommonAreas).IsModified = true;
                        DB.Entry(school).Property(s => s.features.studentLounge).IsModified = true;
                        DB.Entry(school).Property(s => s.features.studentLounge24hour).IsModified = true;
                        DB.Entry(school).Property(s => s.features.englishStaff).IsModified = true;
                        DB.Entry(school).Property(s => s.features.fullTimeJobSupport).IsModified = true;
                        DB.Entry(school).Property(s => s.features.partTimeJobSupport).IsModified = true;
                        DB.Entry(school).Property(s => s.features.interactWithJapanese).IsModified = true;
                        DB.Entry(school).Property(s => s.features.smallClassSizes).IsModified = true;
                        DB.Entry(school).Property(s => s.features.studentDorms).IsModified = true;
                        DB.Entry(school).Property(s => s.features.uniqueFeature).IsModified = true;

                    }
                    

                    if (featuresImage != null && featuresImage.ContentLength > 0)
                    {

                        string path = "/content/images/uploads/schools/" + schoolID;
                        bool exists = Directory.Exists(Server.MapPath(path));
                        if (!exists)
                        {
                            Directory.CreateDirectory(Server.MapPath(path));
                        }

                        path = path + "/featuresImage" + Path.GetExtension(featuresImage.FileName);
                        featuresImage.SaveAs(Server.MapPath(path));
                        school.featuresImage = path;
                        DB.Entry(school).Property(s => s.featuresImage).IsModified = true;
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

                data.message = "school features updated";
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to update school features; " + ex.Message;
                return Json(data);
            }
        }


        [HttpPost, ValidateInput(false)]
        [Route("api/addLocation/{callback?}")]
        public ActionResult addLocation(Location location, string callback,  string description, string city, HttpPostedFileWrapper image)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    
                    
                    DB.Locations.Add(location);
                    DB.SaveChanges();


                    //add name field
                    //Content content = new Content();
                    //content.name = "name";
                    //content.locationID = location.id;
                    //DB.Content.Add(content);
                    //DB.SaveChanges();

                    //ContentBody cb = new ContentBody();
                    //cb.contentID = content.contentID;
                    //cb.code = "en";
                    //cb.body = name;
                    //cb.lastModifiedByID = User.Identity.GetUserId();
                    //cb.lastModified = DateTime.Now;
                    //DB.ContentBody.Add(cb);
                    //DB.SaveChanges();


                    //add description field
                    Content content = new Content();
                    content.name = "description";
                    content.locationID = location.id;
                    DB.Content.Add(content);
                    DB.SaveChanges();

                    ContentBody cb = new ContentBody();
                    cb.contentID = content.contentID;
                    cb.code = "en";
                    cb.body = description;
                    cb.lastModifiedByID = User.Identity.GetUserId();
                    cb.lastModified = DateTime.Now;
                    DB.ContentBody.Add(cb);
                    DB.SaveChanges();

                    ////add city field
                    //content = new Content();
                    //content.name = "city";
                    //content.locationID = location.id;
                    //DB.Content.Add(content);
                    //DB.SaveChanges();

                    //cb = new ContentBody();
                    //cb.contentID = content.contentID;
                    //cb.code = "en";
                    //cb.body = city;
                    //cb.lastModifiedByID = User.Identity.GetUserId();
                    //cb.lastModified = DateTime.Now;
                    //DB.ContentBody.Add(cb);
                    //DB.SaveChanges();


                    if (image != null && image.ContentLength > 0)
                    {

                        string path = "/content/images/uploads/locations/" + location.id;
                        bool exists = Directory.Exists(Server.MapPath(path));
                        if (!exists)
                        {
                            Directory.CreateDirectory(Server.MapPath(path));
                        }

                        path = path + "/locationImage" + Path.GetExtension(image.FileName);
                        image.SaveAs(Server.MapPath(path));
                        location.image = path;
                        DB.Entry(location).Property(l => l.image).IsModified = true;
                    }
                    

                    //int contentID;
                    //contentID = DB.Content.Where(c => c.locationID == location.id && c.name == "name").FirstOrDefault().contentID;

                    //ContentBody cb = DB.ContentBody.Where(c => c.contentID == contentID && c.code == "en").FirstOrDefault();
                    //cb.body = name;

                    
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

                data.message = "location added";
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to add location; " + ex.Message;
                return Json(data);
            }
        }

        [HttpPost]
        [Route("api/getLocations/{code}/{callback?}")]
        public ActionResult getLocations(string callback)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    List<Location> locations = (from l in DB.Locations
                                                select new { l.image, l.locationGallery, l.id, l.name, l.city }).ToList()
                                 .Select(l => new Location { locationGallery = l.locationGallery, id = l.id, image = l.image, name = l.name, city = l.city }).ToList<Location>();


                    foreach (Location location in locations)
                    {
                        location.loadContent("en");
                    }
                    data.data = locations;
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
                data.message = "Failed to retrieve locations; " + ex.Message;
                return Json(data);
            }
        }


        [HttpPost]
        [Route("api/getLocation/{locationID}/{code}/{callback?}")]
        public ActionResult getLocation(int locationID, string code, string callback)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    Location location = DB.Locations.Where(l => l.id == locationID).FirstOrDefault();
                    location.loadContent(code);

                    data.data = location;

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
                data.message = "Failed to retrieve school list; " + ex.Message;
                return Json(data);
            }

        }


        [HttpPost, ValidateInput(false)]
        [Route("api/editSchoolLocation/{schoolID}/{locationID}/{callback?}")]
        public ActionResult editLocation(int locationID, int schoolID, string description, Location location, string callback, HttpPostedFileWrapper image)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    location.id = locationID;

                    DB.Locations.Attach(location);
                    DB.Entry(location).State = EntityState.Unchanged;
                    DB.Entry(location).Property(l => l.name).IsModified = true;
                    //DB.Entry(location).Property(l => l.city).IsModified = true;
                    location.addContent(DB, "description", description);


                    if (image != null && image.ContentLength > 0)
                    {

                        string path = "/content/images/uploads/locations/" + locationID;
                        //bool existsfirst = System.IO.Directory.Exists(Server.MapPath(path));
                        bool exists = System.IO.Directory.Exists(Server.MapPath(path));
                        if (!exists)
                        {
                            Directory.CreateDirectory(Server.MapPath(path));
                        }

                        path = path + "/locationImage" + Path.GetExtension(image.FileName);
                        image.SaveAs(Server.MapPath(path));
                        location.image = path;
                        DB.Entry(location).Property(l => l.image).IsModified = true;
                    }
                    DB.SaveChanges();

                    School school = new School();
                    school.id = schoolID;
                    //school.schoolLocation = location;
                    school.locationID = location.id;
                    school.features = new SchoolFeatures();
                    DB.Schools.Attach(school);
                    DB.Entry(school).State = EntityState.Unchanged;
                    DB.Entry(school).Property(s => s.locationID).IsModified = true;

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

                data.message = "location updated";
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to update location; " + ex.Message;
                return Json(data);
            }
        }

        [HttpPost]
        [Route("api/getStations/{schoolID}/{callback?}")]
        public ActionResult getStations(int schoolID, string callback)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {

                    data.data = (from s in DB.Schools
                                 where s.id == schoolID
                                 select new { s.schoolStations }).ToList();

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
                data.message = "Failed to retrieve school station list; " + ex.Message;
                return Json(data);
            }

        }


        [HttpPost]
        [Route("api/addStation/{schoolID}/{callback?}")]
        public ActionResult addStation(SchoolStation station, int schoolID, string callback, HttpPostedFileWrapper image)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {

                    station.schoolID = schoolID;
                    DB.Stations.Add(station);
                    DB.SaveChanges();

                    try
                    {
                        if (image != null && image.ContentLength > 0)
                        {

                            string path = "/content/images/uploads/schools/" + schoolID;
                            bool exists = Directory.Exists(Server.MapPath(path));
                            if (!exists)
                            {
                                Directory.CreateDirectory(Server.MapPath(path));
                            }

                            path = path + "/station" + station.id + Path.GetExtension(image.FileName);
                            image.SaveAs(Server.MapPath(path));
                            station.image = path;
                            DB.Entry(station).Property(s => s.image).IsModified = true;
                        }

                        DB.SaveChanges();

                    }
                    catch(Exception ex)
                    {

                        data.message = "Failed to add station Image, station added; " + ex.Message;
                        data.statusCode = (int)statusCodes.fail;
                        return Json(data);
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

                data.message = "station added";
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to add station; " + ex.Message;
                return Json(data);
            }
        }



        [HttpPost]
        [Route("api/editSchoolStation/{stationID}/{schoolID}/{callback?}")]
        public ActionResult editSchoolStation(int stationID, int schoolID, SchoolStation station, string callback, HttpPostedFileWrapper image)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    station.id = stationID;
                    DB.Stations.Attach(station);
                    DB.Entry(station).State = EntityState.Unchanged;
                    DB.Entry(station).Property(s => s.line).IsModified = true;
                    DB.Entry(station).Property(s => s.name).IsModified = true;
                    DB.Entry(station).Property(s => s.distance).IsModified = true;

                    if (image != null && image.ContentLength > 0)
                    {

                        string path = "/content/images/uploads/schools/" + schoolID;
                        bool exists = Directory.Exists(Server.MapPath(path));
                        if (!exists)
                        {
                            Directory.CreateDirectory(Server.MapPath(path));
                        }

                        path = path + "/station" + station.id + Path.GetExtension(image.FileName);
                        image.SaveAs(Server.MapPath(path));
                        station.image = path;
                        DB.Entry(station).Property(s => s.image).IsModified = true;
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

                data.message = "station updated";
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to update station; " + ex.Message;
                return Json(data);
            }
        }


        [Route("api/deleteSchoolStation/{stationID}/{callback?}")]
        public ActionResult deleteSchoolStation(int stationID, string callback)
        {
            ajaxReturnData data = new ajaxReturnData();

            using (ApplicationDbContext DB = new ApplicationDbContext())
            {
                try
                {
                    SchoolStation doomedStation = new SchoolStation();
                    doomedStation.id = stationID;
                    DB.Stations.Attach(doomedStation);
                    DB.Stations.Remove(doomedStation);

                    DB.SaveChanges();

                    

                    if (string.IsNullOrEmpty(callback))
                    {
                        data.statusCode = (int)statusCodes.success;
                    }
                    else
                    {
                        data.statusCode = (int)statusCodes.successRun;
                        data.callback = callback;
                    }

                    data.message = "station removed";
                    return Json(data);
                }
                catch (Exception ex)
                {
                    data.statusCode = (int)statusCodes.fail;
                    data.message = "Error: " + ex.Message;
                    return Json(data);
                }

            }
        }


        [Route("api/deleteLocation/{locationID}/{callback?}")]
        public ActionResult deleteLocation(int locationID, string callback)
        {
            ajaxReturnData data = new ajaxReturnData();

            using (ApplicationDbContext DB = new ApplicationDbContext())
            {
                try
                {

                    

                    List<School> schools = DB.Schools.Where(s => s.locationID == locationID).ToList<School>();
                    foreach (School  school in schools)
                    {
                        school.locationID = null;
                        DB.SaveChanges();
                    }

                    List<Content> contents = DB.Content.Where(c => c.locationID == locationID).ToList<Content>();
                    foreach (Content content in contents)
                    {
                        content.locationID = null;
                        DB.SaveChanges();
                    }



                    Location doomedLocation = new Location();
                    doomedLocation.id = locationID;
                    DB.Locations.Attach(doomedLocation);
                    DB.Locations.Remove(doomedLocation);
                    DB.SaveChanges();
                    



                    if (string.IsNullOrEmpty(callback))
                    {
                        data.statusCode = (int)statusCodes.success;
                    }
                    else
                    {
                        data.statusCode = (int)statusCodes.successRun;
                        data.callback = callback;
                    }

                    data.message = "location removed";
                    return Json(data);
                }
                catch (Exception ex)
                {
                    data.statusCode = (int)statusCodes.fail;
                    data.message = "Error: " + ex.Message;
                    return Json(data);
                }

            }
        }



        [Route("api/getSchoolByUrl/{url}/{callback?}")]
        public ActionResult getSchoolByUrl(string url, string callback)
        {
            ajaxReturnData data = new ajaxReturnData();

            using (ApplicationDbContext DB = new ApplicationDbContext())
            {
                try
                {

                    data.data = DB.Schools.Where(s => s.url == url);
                    

                    if (string.IsNullOrEmpty(callback))
                    {
                        data.statusCode = (int)statusCodes.success;
                    }
                    else
                    {
                        data.statusCode = (int)statusCodes.successRun;
                        data.callback = callback;
                    }

                    //data.message = "station removed";
                    return Json(data);
                }
                catch (Exception ex)
                {
                    data.statusCode = (int)statusCodes.fail;
                    data.message = "Error: " + ex.Message;
                    return Json(data);
                }

            }
        }





        [Route("api/getSchoolContent/{schoolID}/{callback?}")]
        public ActionResult getSchoolContent(int schoolID, string callback)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {

                    data.data = DB.Content.Where(c => c.schoolID == schoolID).ToList();

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
                data.message = "Failed to retrieve school content; " + ex.Message;
                return Json(data);
            }

        }



        [HttpPost, ValidateInput(false)]
        [Route("api/addCourseGeneral/{schoolID}/{callback?}")]
        public ActionResult addCourseGeneral(int schoolID, string callback, HttpPostedFileWrapper coursesImage, string coursesHeading)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {

                    School school = DB.Schools.Where(s => s.id == schoolID).FirstOrDefault();
                    school.addContent(DB, "coursesHeading", coursesHeading);
                    DB.SaveChanges();

                   


                    if (coursesImage != null && coursesImage.ContentLength > 0)
                    {

                        string path = "/content/images/uploads/schools/" + schoolID;
                        //bool existsfirst = System.IO.Directory.Exists(Server.MapPath(path));
                        bool exists = Directory.Exists(Server.MapPath(path));
                        if (!exists)
                        {
                            Directory.CreateDirectory(Server.MapPath(path));
                        }

                        path = path + "/coursesImage" + Path.GetExtension(coursesImage.FileName);
                        coursesImage.SaveAs(Server.MapPath(path));
                        school.coursesImage = path;
                        DB.Entry(school).Property(s => s.coursesImage).IsModified = true;
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

                data.message = "school updated";
                return Json(data);
            }
            catch (Exception ex)
            {
                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to update school; " + ex.Message;
                return Json(data);
            }
        }

       
        [Route("api/getSchools/")]
        public ActionResult getSchools(string languageCode)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {

                    List<School> schools = DB.Schools.Include("schoolLocation").ToList();
                    foreach (School school in schools)
                    {
                        school.languageCode = languageCode;
                        school.loadContent(DB);
                    }
                    data.data = schools;
                }


                data.statusCode = (int)statusCodes.success;
                return Json(data);

            }
            catch (Exception ex)
            {

                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to retrieve schools; " + ex.Message;
                return Json(data);
            }

        }



    }
}