using Api.BusinessLogic;
using Api.Models.myModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Diplomski.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class HomeController : ApiController
    {
        public Home home = new Home();


        [HttpPost]
        public HomeModel GetHome(int user_id) => home.GetHome(user_id);

        [HttpPost]
        public SharedModel SetDepartmentML(SearchModel model) => home.SetDepartmentML(model);

        [HttpPost]
        public SharedModel SetCategoryML(SearchModel model) => home.SetCategoryML(model);

        [HttpPost]
        public List<DepartmentModel> GetDepartments(int user_id) => home.GetDepartments(user_id);

        [HttpPost]
        public List<CategoryModel> GetCategories(int user_id) => home.GetCategories(user_id);

        [HttpPost]
        public List<ProductModel> GetRow1Products_1(int user_id) => home.GetRow1Products_1(user_id);
        [HttpPost]
        public List<ProductModel> GetRow1Products_2(int user_id) => home.GetRow1Products_2(user_id);

        [HttpPost]
        public HomeRow2Model GetRow2(int user_id) => home.GetRow2(user_id);

        [HttpPost]
        public List<ProductModel> GetRow3(int user_id,int department_id) => home.GetRow3(user_id, department_id);

        [HttpPost]
        public HomeRow4Model GetProductsForTopCategory(int user_id, int category_id) => home.GetProductsForTopCategory(user_id, category_id);

        [HttpPost]
        public HomeRow5Model GetRow5(int user_id) => home.GetRow5(user_id);


        [HttpPost]
        public List<ProductModel> GetRow6(SearchModel model) => home.GetRow6(model);
    }
}
