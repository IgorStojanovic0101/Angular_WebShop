using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models.myModels
{
    public class DepartmentModel
    {
        public int RecordPk;
        public string DepartmentName;
        public byte[] ImageData { get; set; }
        public string slika { get; set; }

        public List<CategoryModel> CategoryList;

        public float Score;
    }
}