using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SOAP.Models;
using System.Web.Script.Serialization;

namespace SOAP.Controllers
{
    public class HomeController : Controller
    {
        private Service service = new Service();

        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            return View();
        }

        [HttpPost]
        public ActionResult DoLogin(MembershipInfo user)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                ASFUser returnUser = service.GetUser(user);
                if (returnUser.Username == null)
                    dict["success"] = false;
                else
                {
                    if (PasswordHash.ValidatePassword(user.Password, returnUser.Member.Password))
                    {
                        dict["success"] = true;
                        dict["returnUser"] = new JavaScriptSerializer().Serialize(returnUser);
                    }
                    else
                    {
                        dict["success"] = false;
                    }
                }
            }
            catch
            {
                dict["success"] = false;
            }
            return Json(dict);
        }

        [HttpPost]
        public ActionResult RegisterUser(ASFUser user)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                user.Member.Password = PasswordHash.CreateHash(user.Member.Password);
                dict["success"] = service.CreateASFUser(user);
            }
            catch
            {
                dict["success"] = false;
            }
            return Json(dict);
        }

        [HttpPost]
        public ActionResult DeleteUser(ASFUser user)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                service.DeleteASFUser(user);
                dict["success"] = true;
            }
            catch
            {
                dict["success"] = false;
            }
            return Json(dict);
        }

        [HttpPost]
        public ActionResult EditProfile(ASFUser user)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                service.SaveASFUser(user);
                dict["success"] = true;
            }
            catch
            {
                dict["success"] = false;
            }
            return Json(dict);
        }

        [HttpPost]
        public ActionResult GetAllDropdownCategories()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                dict["DropdownCategories"] = service.GetDropdownCategoriesWithValues();
                dict["success"] = true;
            }
            catch
            {
                dict["success"] = false;
            }
            return Json(dict);
        }

    }
}
