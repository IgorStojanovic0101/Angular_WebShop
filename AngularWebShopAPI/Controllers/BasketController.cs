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
    public class BasketController : ApiController
    {

        public Basket basket = new Basket();

        [HttpPost]
        public BasketModel GetBacket(int user_id) => basket.GetBasket(user_id);
        [HttpPost]
        public BasketModel GetBasketById(int pk) => basket.GetBasketById(pk);
        
        [HttpPost]
        public BasketModel SetBacket(BasketModel basket) => this.basket.SetBasket(basket);
    }
}