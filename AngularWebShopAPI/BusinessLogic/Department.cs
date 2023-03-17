using Api.Models.myDatabase;
using Api.Models.myModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Api.BusinessLogic
{
    public class Department : Base
    {

        public Department() : base(null) { }

        public Department(mydatabaseEntities _dbContext)
             : base(_dbContext)
        { }

        public NavModel GetNavBar(int userId)
        {
            var navModel = new NavModel();

            navModel.departments = dBContext.tb_department.Select(x => new DepartmentModel
            {
                DepartmentName = x.department_name,
                RecordPk = x.department_pk
            }).ToList();


            foreach (var item in navModel.departments)
            {
                item.CategoryList = dBContext.tb_category.Where(x => x.category_department_fk.Equals(item.RecordPk))
                    .Select(x => new CategoryModel
                    {
                        CategoryName = x.category_name,
                        DepartmentFk = x.category_department_fk,
                        RecordPk = x.category_pk
                    }).ToList();
            }

            navModel.userName = dBContext.tb_users.Single(x => x.user_pk == userId).user_firstName;

            return navModel;
        }

        public List<DepartmentModel> GetDepartments()
        {
            return dBContext.tb_department.Select(x => new DepartmentModel
            {
                DepartmentName = x.department_name,
                RecordPk = x.department_pk
            }).ToList();
        }

        public List<CategoryModel> GetCategories(int department_id)
        {
            return dBContext.tb_category.Where(x => x.category_department_fk.Equals(department_id))
                    .Select(x => new CategoryModel
                    {
                        CategoryName = x.category_name,
                        DepartmentFk = x.category_department_fk,
                        RecordPk = x.category_pk
                    }).ToList();
        }
        public List<CategoryModel> GetAllCategories()
        {
            return dBContext.tb_category
                    .Select(x => new CategoryModel
                    {
                        CategoryName = x.category_name,
                        DepartmentFk = x.category_department_fk,
                        RecordPk = x.category_pk
                    }).ToList();
        }

        public SharedModel SetDepartment(DepartmentModel dept)
        {
            var returnModel = new SharedModel();

            returnModel.errors = new List<string>();

            var addDepartment = new tb_department
            {
                department_name = dept.DepartmentName,
                start_date = DateTime.Now,
                createdby = "Igor",
                createdondate = DateTime.Now

            };
            dBContext.tb_department.Add(addDepartment);

            dBContext.SaveChanges();


            string output = dept.slika.Substring(dept.slika.IndexOf(',') + 1);
            var addImage = new tb_dept_cat_images
            {
                dept_cat_department_fk = addDepartment.department_pk,
                dept_cat_category_fk = null,
                dept_cat_image_data = Convert.FromBase64String(output),
                start_date = DateTime.Now,
                createdby = "Igor",
                createdondate = DateTime.Now

            };

            dBContext.tb_dept_cat_images.Add(addImage);
            dBContext.SaveChanges();

            returnModel.Status = Models.myEnums.CreateStatus.sucess;
            return returnModel;
        }

        public SharedModel SetCategory(CategoryModel cat)
        {
            var returnModel = new SharedModel();

            returnModel.errors = new List<string>();

            var addCategory = new tb_category
            {
                category_name = cat.CategoryName,
                category_department_fk = cat.DepartmentFk,
                start_date = DateTime.Now,
                createdby = "Igor",
                createdondate = DateTime.Now

            };
            dBContext.tb_category.Add(addCategory);

            dBContext.SaveChanges();


            string output = cat.slika.Substring(cat.slika.IndexOf(',') + 1);
            var addImage = new tb_dept_cat_images
            {
                dept_cat_department_fk = null,
                dept_cat_category_fk = addCategory.category_pk,
                dept_cat_image_data = Convert.FromBase64String(output),
                start_date = DateTime.Now,
                createdby = "Igor",
                createdondate = DateTime.Now

            };

            dBContext.tb_dept_cat_images.Add(addImage);
            dBContext.SaveChanges();

            returnModel.Status = Models.myEnums.CreateStatus.sucess;
            return returnModel;
        }

    }
}