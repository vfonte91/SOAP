using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SOAP.Models;

namespace SOAP.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            SampleService s = new SampleService();
            List<DropdownCategory> ddowns = s.SelectQuery();
            return View();
        }

    }
}
