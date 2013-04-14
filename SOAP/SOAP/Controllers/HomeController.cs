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
        public ActionResult Export(Patient pat)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                service.Export();
            }
            catch
            {

            }
            return Json(dict);
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
            catch(Exception e)
            {
                dict["success"] = false;
                dict["error"] = e.Message;
            }
            return Json(dict);
        }

        [HttpPost]
        public ActionResult GetUsers()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                List<ASFUser> users = service.GetASFUsers();
                dict["succes"] = true;
                dict["users"] = users;
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
        public ActionResult PromoteUser(ASFUser user)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                service.Promote(user);
                dict["success"] = true;
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
        public ActionResult CreateForm(Patient pat)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                service.CreatePatient(pat);
                dict["success"] = true;
            }
            catch (Exception e)
            {
                service.DeletePatient(pat);
                dict["success"] = false;
                dict["message"] = e.Message;
                dict["stacktrace"] = e.StackTrace;
            }
            return Json(dict);
        }

        [HttpPost]
        public ActionResult SaveForm(Patient pat)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                service.SavePatient(pat);
                dict["success"] = true;
            }
            catch (Exception e)
            {
                dict["success"] = false;
                dict["message"] = e.Message;
                dict["stacktrace"] = e.StackTrace;
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

        [HttpPost]
        public ActionResult GetDropdownValues(DropdownCategory catId)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                dict["DropdownValues"] = service.GetDropdownValues(catId.Id);
                dict["success"] = true;
            }
            catch
            {
                dict["success"] = false;
            }
            return Json(dict);
        }

        [HttpPost]
        public ActionResult GetUserForms(ASFUser user)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                dict["Forms"] = service.GetForms(user);
                dict["success"] = true;
            }
            catch
            {
                dict["success"] = false;
            }
            return Json(dict);
        }

        [HttpPost]
        public ActionResult GetPatient(Patient form)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                dict["Patient"] = service.GetPatient(form.PatientId);
                dict["success"] = true;
            }
            catch (Exception e)
            {
                dict["success"] = false;
                dict["message"] = e.Message;
                dict["source"] = e.StackTrace;
            }
            return Json(dict);
        }

        [HttpPost]
        public ActionResult GetSecurityQuestion(ASFUser user)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                dict["securityQuestion"] = service.GetSecurityQuestion(user.Username);
                dict["success"] = true;
            }
            catch
            {
                dict["success"] = false;
            }
            return Json(dict);
        }

        [HttpPost]
        public ActionResult CheckSecurityAnswer(ASFUser user)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                dict["success"] = service.CheckSecurityAnswer(user.Username, user.Member.SecurityAnswer);
            }
            catch
            {
                dict["success"] = false;
            }
            return Json(dict);
        }

        [HttpPost]
        public ActionResult ChangeForgottenPassword(ASFUser user)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                user.Member.Password = PasswordHash.CreateHash(user.Member.Password);
                service.ChangeForgottenPassword(user);
                dict["success"] = true;
            }
            catch
            {
                dict["success"] = false;
            }
            return Json(dict);
        }

        [HttpPost]
        public ActionResult ChangePassword(ASFUser user)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                user.Member.Password = PasswordHash.CreateHash(user.Member.Password);
                service.ChangePassword(user, user.Member.OldPassword, user.Member.Password);
                dict["success"] = true;
            }
            catch
            {
                dict["success"] = false;
            }
            return Json(dict);
        }

        [HttpPost]
        public ActionResult EditDropdownValue(DropdownValue dropdown)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                service.SaveDropdownValue(dropdown);
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
