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

namespace GoGoNihon_MVC.Controllers
{
    public struct userList
    {
        public object roles;
        public object users;
        public object languages;
        public object userLanguages;

    }


    public struct role {

        public string Name;
        public string Id;
    }

    public struct user {
        public string Id;
        public string PhoneNumber;
        public ICollection<IdentityUserRole> Roles;
        public string UserName;
        public string Email;
    }

    [Authorize(Roles = "admin")]
    public class APIUserController : Controller
    {
        [Route("api/getUsers/{callback?}")]
        public ActionResult getUsers()
        {
            ajaxReturnData data = new ajaxReturnData();
            userList userlist = new userList();

            using (ApplicationDbContext DB = new ApplicationDbContext())
            {
                
                userlist.roles = (from r in DB.Roles
                             select new { Name = r.Name, Id = r.Id }).ToList();

                userlist.users = (from u in DB.Users
                                  select new {u.Roles, u.UserName, u.Email, u.Id, u.PhoneNumber}).ToList();

                userlist.languages = (from l in DB.Language
                                  select new {l.name, l.code, l.flag }).ToList();
                userlist.userLanguages = (from l in DB.UserLanguages
                                      select new { l.userID, l.code, l.user, l.userLanguageID}).ToList();

                data.data = userlist;
            }

            data.statusCode = (int)statusCodes.successRun;
            return Json(data);
        }

        [Route("api/getRoles/{callback?}")]
        public ActionResult getRoles()
        {
            ajaxReturnData data = new ajaxReturnData();

            using (ApplicationDbContext DB = new ApplicationDbContext())
            {

                data.data = (from r in DB.Roles
                                  select new { Name = r.Name, Id = r.Id }).ToList();
               
            }

            data.statusCode = (int)statusCodes.successRun;
            return Json(data);
        }

        [Route("api/addRole/{userID}/{roleID}")]
        public async Task<ActionResult> addRole(string userID, string roleID)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    ApplicationUser user = new ApplicationUser();

                    var store = new UserStore<ApplicationUser>(DB);
                    UserManager<ApplicationUser> um = new UserManager<ApplicationUser>(store);
                    user = um.FindById(userID);
                    var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(DB));
                    IdentityRole role = RoleManager.FindById(roleID);
                    await store.AddToRoleAsync(user, role.Name);
                    await store.UpdateAsync(user);

                    data.statusCode = (int)statusCodes.successRun;
                    data.message = "Role '" + role.Name + "' added to user '" + user.UserName + "'";

                }

            }
            catch(Exception ex)
            {

                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to add role to user; Error is: "+ ex.Message;
            }
            

            
            return Json(data);
        }

        [Route("api/addNewRole/")]
        [ValidateAntiForgeryToken]
        public ActionResult addNewRole(string name)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                if (!String.IsNullOrWhiteSpace(name))
                {
                    using (ApplicationDbContext DB = new ApplicationDbContext())
                    {
                        if (!DB.Roles.Any(r => r.Name == name))
                        {
                            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(DB));
                            IdentityRole newRole = new IdentityRole();
                            newRole.Name = name;
                            roleManager.Create(newRole);

                            data.statusCode = (int)statusCodes.successRun;
                            data.callback = "loadRoleListAfterChanges";
                            data.message = "role added";

                        }
                        else
                        {
                            data.statusCode = (int)statusCodes.fail;
                            data.message = "role already exists genius";

                        }

                    }
                }
                else
                {
                    data.statusCode = (int)statusCodes.fail;
                    data.message = "roles need names...";
                }
                

            }
            catch (Exception ex)
            {

                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to add role to user; Error is: " + ex.Message;
            }



            return Json(data);
        }

        [Route("api/deleteRole/{roleID}")]
        public ActionResult deleteRole(string roleID)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    ApplicationUser user = new ApplicationUser();

                    
                    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(DB));
                    IdentityRole role = roleManager.FindById(roleID);
                    string name = role.Name;
                    roleManager.Delete(role);

                    data.statusCode = (int)statusCodes.successRun;
                    data.message = "Role '" + name + "' removed";

                }

            }
            catch (Exception ex)
            {

                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to remove role; Error is: " + ex.Message;
            }


            return Json(data);
        }


        //removes a role from a USER
        [Route("api/removeRole/{userID}/{roleID}")]
        public async Task<ActionResult> removeRole(string userID, string roleID)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    ApplicationUser user = new ApplicationUser();

                    var store = new UserStore<ApplicationUser>(DB);
                    UserManager<ApplicationUser> um = new UserManager<ApplicationUser>(store);
                    user = um.FindById(userID);
                    var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(DB));
                    IdentityRole role = RoleManager.FindById(roleID);

                    if (user.UserName == "nathan@gogoworld.com" && role.Name == "admin") {
                        data.statusCode = (int)statusCodes.fail;
                        data.message = "I'm sorry Dave, I'm afraid I can't do that";
                    }
                    else
                    {
                        await store.RemoveFromRoleAsync(user, role.Name);
                        await store.UpdateAsync(user);

                        data.statusCode = (int)statusCodes.successRun;
                        data.message = "Role '" + role.Name + "' removed from user '" + user.UserName + "'";

                    }
                    

                }

            }
            catch (Exception ex)
            {

                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to remove role from user; Error is: " + ex.Message;
            }
            

            return Json(data);
        }


        [Route("api/deleteUser/{userID}/")]
        public ActionResult deleteUser(string userID)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    ApplicationUser user = new ApplicationUser();

                    var store = new UserStore<ApplicationUser>(DB);
                    UserManager<ApplicationUser> um = new UserManager<ApplicationUser>(store);
                    user = um.FindById(userID);
                    if (user.UserName == "nathan@gogoworld.com")
                    {
                        data.statusCode = (int)statusCodes.fail;
                        data.message = "I'm sorry Dave, I'm afraid I can't do that";
                    }
                    else {
                        DB.UserLanguages.RemoveRange(DB.UserLanguages.Where(x => x.userID == userID));
                        data.message = "User '" + user.UserName + "' has become one with the void";
                        um.Delete(user);
                        data.statusCode = (int)statusCodes.successRun;
                    }
                    
                }

            }
            catch (Exception ex)
            {

                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to delete user; Error is: " + ex.Message;
            }



            return Json(data);
        }


        [Route("api/assignLanguage/{code}/{userID}")]
        public ActionResult assignLanguage(string userID, string code)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    
                    if (!DB.UserLanguages.Any(u => u.userID == userID && u.code == code))
                    {
                        UserLanguage ul = new UserLanguage();

                        ul.userID = userID;
                        ul.code = code;
                        DB.UserLanguages.Add(ul);
                        DB.SaveChanges();

                        data.statusCode = (int)statusCodes.successRun;
                        data.message = "language added to user";
                        
                    }
                    else
                    {
                        data.statusCode = (int)statusCodes.fail;
                        data.message = "Language already exists for user";

                    }

                }

            }
            catch (Exception ex)
            {

                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to add language to user; Error is: " + ex.Message;
            }
            
            return Json(data);
        }


        [Route("api/unassignLanguage/{code}/{userID}")]
        public ActionResult unassignLanguage(string userID, string code)
        {
            ajaxReturnData data = new ajaxReturnData();

            try
            {
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {

                    if (DB.UserLanguages.Where(u => u.code == code && u.userID == userID).Any()) {
                        UserLanguage ul = DB.UserLanguages.Where(u => u.code == code && u.userID == userID).First();
                        DB.UserLanguages.Remove(ul);
                        DB.SaveChanges();
                        data.statusCode = (int)statusCodes.successRun;
                        data.message = "language removed from user";
                    }
                    else
                    {
                        data.statusCode = (int)statusCodes.fail;
                        data.message = "computer says that user didnt have that language anyway...";
                    }
                    

                }

            }
            catch (Exception ex)
            {

                data.statusCode = (int)statusCodes.fail;
                data.message = "Failed to remove language from user; Error is: " + ex.Message;
            }


            return Json(data);
        }



    }
}