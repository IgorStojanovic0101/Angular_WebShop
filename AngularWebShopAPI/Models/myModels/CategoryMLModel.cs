using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models.myModels
{
    public class CategoryMLModel
    {
        public int RecordPk;
        public int CategoryFk;
        public int UserFk;
        public float ItemCount;
        public float ModifiedItemCount;
    }
}