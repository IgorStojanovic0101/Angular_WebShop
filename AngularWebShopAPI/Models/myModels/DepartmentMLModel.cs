using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models.myModels
{
    public class DepartmentMLModel
    {
        public int RecordPk;
        public int DepartmentFk;
        public int UserFk;
        public float ItemCount;
        public float ModifiedItemCount;
    }
}