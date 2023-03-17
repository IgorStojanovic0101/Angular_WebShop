using Api.Models.myEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models.myModels
{
    public class BasketModel
    {
        public int BasketPk;
        public int UserFk;
        public List<BasketItemModel> itemList;
        public BasketStatus status;

        
    }
}