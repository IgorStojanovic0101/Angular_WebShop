using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Api.Controllers
{
    public class IndexController : Controller
    {


        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
    }
}