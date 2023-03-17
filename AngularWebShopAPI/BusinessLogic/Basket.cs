using Api.Models.myDatabase;
using Api.Models.myEnums;
using Api.Models.myModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Api.BusinessLogic
{
    public class Basket : Base
    {

        public Basket() : base(null) { }

        public Basket(mydatabaseEntities _dbContext)
             : base(_dbContext)
        { }
        public BasketModel GetBasket(int userId)
        {

            var anyBasket = dBContext.tb_basket.Any(x => x.basket_user_fk == userId);

            if (anyBasket)
            {
                var basket = dBContext.tb_basket.Where(x => x.basket_user_fk == userId)
                .Select(x => new BasketModel
                {
                    BasketPk = x.basket_pk,
                    UserFk = x.basket_user_fk
                }).SingleOrDefault();

                basket.status = BasketStatus.Valid;

                if (basket != null)
                {
                    basket.itemList = new List<BasketItemModel>();

                    basket.itemList = dBContext.tb_basket_item.Where(x => x.basket_item_basket_fk == basket.BasketPk).Select(x => new BasketItemModel
                    {
                        BasketItemPk = x.basket_item_pk,
                        BasketFk = x.basket_item_basket_fk,
                        ProductFk = x.basket_item_product_fk,
                        ProductName = x.basket_item_product_name,
                        ProductPrice = x.basket_item_product_price,
                        ItemCount = x.basket_item_count

                    }).ToList();

                    foreach (var item in basket.itemList)
                    {
                        var image = dBContext.tb_product_images.SingleOrDefault(x => x.prod_id == item.ProductFk && x.prod_main_photo);
                        item.slika = "data:image/jpg;base64," + System.Convert.ToBase64String(image.prod_image_data);
                    }
                }
                return basket;
            }
            else
            {
                var newBasket = new tb_basket
                {
                    basket_user_fk = userId,
                    createdondate = Convert.ToDateTime(DateTime.Now, cultureInfo),
                    start_date = Convert.ToDateTime(DateTime.Now, cultureInfo),
                    createdby = "Igor"
                };
                dBContext.tb_basket.Add(newBasket);
                dBContext.SaveChanges();

                var basket = new BasketModel();

                basket.itemList = new List<BasketItemModel>();
                basket.BasketPk = newBasket.basket_pk;
                basket.UserFk = userId;
                basket.status = BasketStatus.Valid;


                return basket;

            }

        }
        public BasketModel GetBasketById(int pk)
        {

          
                var basket = dBContext.tb_basket.Where(x => x.basket_pk == pk).Select(x => new BasketModel
                {
                    BasketPk = x.basket_pk,
                    UserFk = x.basket_user_fk
                }).SingleOrDefault();

                basket.status = BasketStatus.Valid;

                if (basket != null)
                {
                    basket.itemList = new List<BasketItemModel>();

                    basket.itemList = dBContext.tb_basket_item.Where(x => x.basket_item_basket_fk == basket.BasketPk).Select(x => new BasketItemModel
                    {
                        BasketItemPk = x.basket_item_pk,
                        BasketFk = x.basket_item_basket_fk,
                        ProductFk = x.basket_item_product_fk,
                        ProductName = x.basket_item_product_name,
                        ProductPrice = x.basket_item_product_price,
                        ItemCount = x.basket_item_count

                    }).ToList();

                    foreach (var item in basket.itemList)
                    {
                        var image = dBContext.tb_product_images.SingleOrDefault(x => x.prod_id == item.ProductFk && x.prod_main_photo);
                        item.slika = "data:image/jpg;base64," + System.Convert.ToBase64String(image.prod_image_data);
                    }
                }
                return basket;
           

        }

        public BasketModel SetBasket(BasketModel basket)
        {
                
            var itemsDb = dBContext.tb_basket_item.Where(x => x.basket_item_basket_fk == basket.BasketPk).Select(x => x.basket_item_pk).ToList();
            var itemsList = basket.itemList.Select(x => x.BasketItemPk).ToList();

            switch (basket.status)
            {
                

                case BasketStatus.DeleteItem:
                    {
                        
                        var differences = itemsDb.Except(itemsList).ToList();

                        var itemToRemove = dBContext.tb_basket_item.FirstOrDefault(x => differences.Contains(x.basket_item_pk));
                        if (itemToRemove != null)
                            dBContext.tb_basket_item.Remove(itemToRemove);

                        
                        dBContext.SaveChanges();

                        break;
                    }
                case BasketStatus.AddUpdateItem:
                    {

                        var differences = itemsList.Except(itemsDb).ToList();
                        var itemToAdd = basket.itemList.FirstOrDefault(x => differences.Contains(x.BasketItemPk));

                        var remove = dBContext.tb_basket_item.SingleOrDefault(x => x.basket_item_basket_fk == itemToAdd.BasketFk && x.basket_item_product_fk == itemToAdd.ProductFk);
                        if (remove != null)
                            dBContext.tb_basket_item.Remove(remove);
                        
                        var newBasketItem = new tb_basket_item
                        {
                            basket_item_basket_fk = basket.BasketPk,
                            basket_item_count = itemToAdd.ItemCount,
                            basket_item_product_name = itemToAdd.ProductName,
                            basket_item_product_price = itemToAdd.ProductPrice,
                            basket_item_product_fk = itemToAdd.ProductFk,
                            createdby = "Igor",
                            createdondate = Convert.ToDateTime(DateTime.Now, cultureInfo),
                            start_date = Convert.ToDateTime(DateTime.Now, cultureInfo)

                        };
                        dBContext.tb_basket_item.Add(newBasketItem);
                        // dBContext.SaveChanges();

                     

                        var newProductSale = new tb_product_sale
                        {
                            product_sale_product_fk = itemToAdd.ProductFk,
                            product_sale_count = itemToAdd.ItemCount,
                            product_sale_user_fk = basket.UserFk,
                            createdby = "Igor",
                            createdondate = Convert.ToDateTime(DateTime.Now, cultureInfo),
                            start_date = Convert.ToDateTime(DateTime.Now, cultureInfo)
                        };
                        dBContext.tb_product_sale.Add(newProductSale);
                        dBContext.SaveChanges();

                        basket.itemList.SingleOrDefault(x => x.ProductFk == itemToAdd.ProductFk).BasketItemPk = newBasketItem.basket_item_pk;

                        break;
                    }

                case BasketStatus.UpdateItem:
                {
                        foreach (var item in basket.itemList)
                        {
                            var ChangedbasketItem = dBContext.tb_basket_item.SingleOrDefault(x => x.basket_item_basket_fk == item.BasketFk && x.basket_item_product_fk == item.ProductFk && x.basket_item_count != item.ItemCount);
                            if (ChangedbasketItem != null)
                            {
                                ChangedbasketItem.basket_item_count = item.ItemCount;
                                ChangedbasketItem.lastupdatedby = "Igor";
                                ChangedbasketItem.lastupdateddate = Convert.ToDateTime(DateTime.Now, cultureInfo);

                                dBContext.SaveChanges();
                            }
                            var ChangedProductSaleItem = dBContext.tb_product_sale.SingleOrDefault(x => x.product_sale_user_fk ==  basket.UserFk && x.product_sale_product_fk == item.ProductFk && x.product_sale_count != item.ItemCount);
                            if (ChangedProductSaleItem != null)
                            {
                                ChangedProductSaleItem.product_sale_count = item.ItemCount;
                                ChangedProductSaleItem.lastupdatedby = "Igor";
                                ChangedProductSaleItem.lastupdateddate = Convert.ToDateTime(DateTime.Now, cultureInfo);

                                dBContext.SaveChanges();
                            }
                        }
                        break;
                }

                case BasketStatus.DeleteItem_DeleteBasket:
                    {

                        var removeItem = dBContext.tb_basket_item.First(x => x.basket_item_basket_fk == basket.BasketPk);
                        if (removeItem != null)
                            dBContext.tb_basket_item.Remove(removeItem);

                        /*var removeBasket = dBContext.tb_basket.First(x => x.basket_pk == basket.BasketPk);
                        if (removeBasket != null)
                            dBContext.tb_basket.Remove(removeBasket);*/

                        dBContext.SaveChanges();


                        break;
                    }

                case BasketStatus.DeleteAllItemsInBasket:
                    {
                        var IdsToRemove = dBContext.tb_basket_item.Where(x => x.basket_item_basket_fk == basket.BasketPk).Select(x=>x.basket_item_pk).ToList();
                        foreach (var id in IdsToRemove)
                        {
                            var removeItem = dBContext.tb_basket_item.SingleOrDefault(x => x.basket_item_pk.Equals(id));
                            if (removeItem != null)
                                dBContext.tb_basket_item.Remove(removeItem);
                        }
                      /*  var removeBasket = dBContext.tb_basket.FirstOrDefault(x => x.basket_pk == basket.BasketPk);
                        if (removeBasket != null)
                            dBContext.tb_basket.Remove(removeBasket);*/

                        dBContext.SaveChanges();

                        break;
                    }


                default: break;
            }


          
            basket.status = BasketStatus.Valid;
            return basket;

        }

            
            
          

    }
}