using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models.myModels
{


   
    public class HomeModel
    {
        public List<DepartmentModel> departmentList;
        public List<CategoryModel> categoryList;
        public int categoryDepartmentFk;
        public List<ProductModel> productsRow1_predictions1;
        public List<ProductModel> productsRow1_predictions2;
        public List<ProductModel> productsRow2_predictions;


    }


}