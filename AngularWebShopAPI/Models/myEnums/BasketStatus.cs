using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models.myEnums
{
    public enum BasketStatus
    {
        CreateBasket_addItem = 1,
        AddUpdateItem = 2,
        UpdateItem = 3,
        DeleteItem = 4,
        DeleteItem_DeleteBasket = 5,
        DeleteAllItemsInBasket = 6,
        Valid = 7


    }
}