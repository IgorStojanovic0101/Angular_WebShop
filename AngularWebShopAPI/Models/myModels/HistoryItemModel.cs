using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models.myModels
{
    public class HistoryItemModel
    {
        public int HistoryItemPk;
        public string ProductName;
        public string slika;
        public int ProductFk;
        public int UserFk;
        public int Rating;
        public float ModifiedRating;
        public int ProductPrice;
        public string[] RatingStar;
        public DateTime? lastUpdate;
        public DateTime createdon;

    }
}