using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models.myModels
{
    public class RatingModel
    {
        public int RatingPk;
        public int ProductFk;
        public int UserFk;
        public float Rating;
    }
}