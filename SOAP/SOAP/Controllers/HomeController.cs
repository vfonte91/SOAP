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
            //ASFUser user1 = new ASFUser();
            //user1.FullName = "Test";
            //user1.EmailAddress = "Email";
            //user1.IsAdmin = 0;
            //user1.Username = "TestUser";
            //MembershipInfo member = new MembershipInfo();
            //member.Username = "TestUser";
            //member.Password = "pass";
            //user1.MembershipInfo = member;
            //return Json(user1);
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
