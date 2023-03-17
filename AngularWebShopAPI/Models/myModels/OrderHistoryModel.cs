using Api.Models.myEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models.myModels
{
    public class OrderHistoryModel
    {
        public int UserFk;
        public List<HistoryItemModel> itemList;
        public HistoryStatus status;
    }
}