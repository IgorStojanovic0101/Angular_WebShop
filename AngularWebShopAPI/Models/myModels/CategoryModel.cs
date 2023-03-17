using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models.myModels
{
    public class CategoryModel
    {
        public int RecordPk;
        public string CategoryName;
        public int DepartmentFk;
        public byte[] ImageData { get; set; }
        public string slika { get; set; }
        public float Score { get; set; }
    }
}