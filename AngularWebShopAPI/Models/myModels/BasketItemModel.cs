using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models.myModels
{
    public class BasketItemModel
    {
        public int BasketItemPk;
        public int BasketFk;
        public int ProductFk;
        public int UserFk;
        public string ProductName;
        public int ProductPrice;
        public int ItemCount;
        public string slika;
    }
}