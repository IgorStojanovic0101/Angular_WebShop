using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models.myModels
{
    public class SearchModel
    {
        public int categoryFk;
        public int departmentFk;
        public int userId;
        public int departmentCount;
        public int categoryCount;
        public List<CategoryModel> categoryList;
        public string search;
    }
}