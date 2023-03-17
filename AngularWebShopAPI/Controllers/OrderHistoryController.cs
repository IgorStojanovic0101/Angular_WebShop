using Api.BusinessLogic;
using Api.Models.myModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class OrderHistoryController : ApiController
    {   
        
       public OrderHistory history = new OrderHistory();

        [HttpPost]
        public OrderHistoryModel GetHistory(int user_id) => history.GetHistory(user_id);

        [HttpPost]
        public OrderHistoryModel SetHistory(OrderHistoryModel model) => history.SetHistory(model);
        [HttpPost]
        public OrderHistoryModel FilterHistoryProducts(int user_id, FilterModel filter) => history.FilterHistoryProducts(user_id, filter);

    }
}