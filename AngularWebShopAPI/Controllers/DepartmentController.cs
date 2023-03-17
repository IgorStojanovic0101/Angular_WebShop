using Api.BusinessLogic;
using Api.Models.myModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class DepartmentController : ApiController
    {
        public Department department = new Department();

        [HttpPost]
        public NavModel GetNavBar(int user_id) => department.GetNavBar(user_id);

        [HttpGet]
        public List<DepartmentModel> GetDepartments() => department.GetDepartments();

        [HttpPost]
        public List<CategoryModel> GetCategories(int department_id) => department.GetCategories(department_id);

        [HttpGet]
        public List<CategoryModel> GetAllCategories() => department.GetAllCategories();

        [HttpPost]
        public SharedModel SetDepartment(DepartmentModel dept) => department.SetDepartment(dept);

        [HttpPost]
        public SharedModel SetCategory(CategoryModel cat) => department.SetCategory(cat);


    }
}