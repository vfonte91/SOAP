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
        public ActionResult DoLogin(ASFUser user)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                ASFUser returnUser = service.DoLogin(user);
                if (returnUser.Username == null)
                    dict["success"] = false;
                else
                {
                    dict["success"] = true;
                    dict["returnUser"] = new JavaScriptSerializer().Serialize(returnUser);
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
                dict["success"] = service.CreateASFUser(user);
            }
            catch
            {
                dict["success"] = false;
            }
            return Json(dict);
        }

    }
}
