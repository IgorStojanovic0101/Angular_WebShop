using Api.Models.myModels;
using Api.Models.Urls;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Klijent.Controllers
{
	public class HomeController : Base
    {
        public ActionResult Index()
        {
            UserModel userModel = new UserModel();
            var result = wsGet<List<UserModel>>(SystemUrls.User.GetUsers);
            userModel.FirstName = "Igor";
            var result2 = wsPost<UserModel, List<UserModel>>(SystemUrls.User.FindUsers, userModel);


            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}