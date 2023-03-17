using Api.Models.myModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Api.Models.myDatabase;
using Api.Models.myEnums;
using ML.NET;
using Microsoft.Ajax.Utilities;

namespace Api.BusinessLogic
{
    public class OrderHistory : Base
    {

        public OrderHistory() : base(null) { }

        public OrderHistory(mydatabaseEntities _dbContext)
             : base(_dbContext)
        { }
        public OrderHistoryModel GetHistory(int userId)
        {
            var orderhistory = new OrderHistoryModel();
            orderhistory.UserFk = userId;
            orderhistory.itemList = dBContext.tb_history_item.Where(x => x.history_item_user_fk == userId)
            .Select(x => new HistoryItemModel
            {
                HistoryItemPk = x.history_item_pk,
                ProductName = x.tb_product.product_name,
                ProductFk = x.history_item_product_fk,
                Rating = x.history_item_rating,
                UserFk = x.history_item_user_fk
            }).ToList();

            foreach (var item in orderhistory.itemList)
            {

                item.RatingStar = FillStarRating(item.HistoryItemPk, item.Rating);

                var image = dBContext.tb_product_images.SingleOrDefault(x => x.prod_id == item.ProductFk && x.prod_main_photo);
                item.slika = "data:image/jpg;base64," + System.Convert.ToBase64String(image.prod_image_data);
            }

            return orderhistory;
        }
        public OrderHistoryModel SetHistory(OrderHistoryModel model)
        {
            var orderHistory = new OrderHistoryModel();
            var mb = new ModelBuilder();           
            
            var sendModel = new SendModel(dBContext.Database.Connection, con_string, typeof(tb_rating));

            orderHistory.UserFk = model.UserFk;


            


            switch (model.status)
            {
                case HistoryStatus.CreateHistory_addItem:
                    {
                        // var ss = model.itemList.Select(x => x.ProductFk).ToList()
                        var Productids = dBContext.tb_history_item.Where(x => x.history_item_user_fk == model.UserFk).Select(x => x.history_item_product_fk).ToList();
                        var any = model.itemList.Any(x => Productids.Contains(x.ProductFk));


                        foreach (var item in model.itemList)
                        {
                            if (!Productids.Contains(item.ProductFk))
                            {
                                var newHistoryItem = new tb_history_item
                                {
                                    //history_item_product_name = item.ProductName,
                                    history_item_rating = item.Rating,
                                    history_item_user_fk = item.UserFk,
                                    history_item_product_fk = item.ProductFk,
                                    createdby = "Igor",
                                    createdondate = Convert.ToDateTime(DateTime.Now, cultureInfo),
                                    start_date = Convert.ToDateTime(DateTime.Now, cultureInfo)

                                };

                                dBContext.tb_history_item.Add(newHistoryItem);

                                var newRating = new tb_rating
                                {
                                    rating_item = item.Rating,
                                    rating_user_fk = item.UserFk,
                                    rating_product_fk = item.ProductFk,

                                };
                                dBContext.tb_rating.Add(newRating);
                            }
                        }
                        if (!any)
                        {
                            dBContext.SaveChanges();
                        }


                        orderHistory.itemList = dBContext.tb_history_item.Where(x => x.history_item_user_fk == model.UserFk)
                                .Select(x => new HistoryItemModel
                                {
                                    HistoryItemPk = x.history_item_pk,
                                    ProductFk = x.history_item_product_fk,
                                    UserFk = x.history_item_user_fk,
                                    Rating = x.history_item_rating
                                }).ToList();
                        foreach (var item in orderHistory.itemList)
                        {
                            item.RatingStar = FillStarRating(item.HistoryItemPk, item.Rating);
                            var image = dBContext.tb_product_images.SingleOrDefault(x => x.prod_id == item.ProductFk && x.prod_main_photo);
                            item.slika = "data:image/jpg;base64," + System.Convert.ToBase64String(image.prod_image_data);
                            // item.HistoryItemPk = orderHistory.itemList.Single(x => x.ProductFk == item.ProductFk).HistoryItemPk;
                        }

                        break;
                    }
                case HistoryStatus.UpdateItem:
                    {
                        orderHistory.itemList = model.itemList;
                        foreach (var item in orderHistory.itemList)
                        {
                            var ChangedHistoryItem = dBContext.tb_history_item.SingleOrDefault(x => x.history_item_pk == item.HistoryItemPk && x.history_item_rating != item.Rating);
                            if (ChangedHistoryItem != null)
                            {
                                item.RatingStar = FillStarRating(item.HistoryItemPk, item.Rating);

                                ChangedHistoryItem.history_item_rating = item.Rating;
                                ChangedHistoryItem.lastupdatedby = "Igor";
                                ChangedHistoryItem.lastupdateddate = Convert.ToDateTime(DateTime.Now, cultureInfo);



                                dBContext.SaveChanges();



                            }
                        }
                       
                      var USERFK = orderHistory.itemList.FirstOrDefault().UserFk;
                      var historyItems =  dBContext.tb_history_item.Where(x =>  x.history_item_user_fk == USERFK)
                      .Select(x=> new HistoryItemModel
                          {
                               HistoryItemPk = x.history_item_pk,
                              ProductFk = x.history_item_product_fk,
                              Rating = x.history_item_rating,
                              lastUpdate = x.lastupdateddate,
                              createdon = x.createdondate         
                          }).ToList();

                        var duplicates = historyItems.GroupBy(s => s.Rating).Where(g => g.Count() > 1).SelectMany(g => g).ToList();

                        var ratingList = duplicates.DistinctBy(i => i.Rating).Select(x=>x.Rating).ToList();
                        foreach(var rating in ratingList)
                        {
                            var fragment = duplicates.Where(x => x.Rating.Equals(rating)).OrderByDescending(x=>x.lastUpdate).ThenBy(x=>x.createdon).ToList();
                            int count = fragment.Count();
                            for (int i = 0; i < count; i++)
                            {
                                var value = rating - (float)(i * 0.1);
                                fragment[i].ModifiedRating = value;
                                historyItems.Single(x => x.HistoryItemPk == fragment[i].HistoryItemPk).ModifiedRating = fragment[i].ModifiedRating;
                            }           
                        }
                        foreach (var item in historyItems)
                        {
                            var RatingChanged = dBContext.tb_rating.Single(x => x.rating_product_fk == item.ProductFk && x.rating_user_fk == USERFK);
                            RatingChanged.rating_item =  item.ModifiedRating != 0 ? item.ModifiedRating:item.Rating;
                        }


                        dBContext.SaveChanges();

                        /*
                        var productIds = dBContext.tb_rating.Where(x => x.rating_user_fk == USERFK).Select(x => x.rating_product_fk).ToList();

                        var rating_list =  dBContext.tb_rating.Where(x => productIds.Contains(x.rating_product_fk) && x.rating_item != 0)
                        .Select(x => new RatingModel
                         {
                             RatingPk = x.rating_pk,
                             ProductFk = x.rating_product_fk,
                             Rating = x.rating_item,
                             UserFk = x.rating_user_fk
                         }).ToList();

                        var duplicates2 = rating_list.GroupBy(s => s.Rating)
                                    .Where(g => g.Count() > 1)
                                    .SelectMany(g => g).ToList();

                        var ratingListUnique = duplicates2.DistinctBy(i => i.Rating).ToList();

                        //var fragment2 = new List<RatingModel>();


                        // else
                        //  fragment2 = duplicates.Where(x => Math.Ceiling(x.ItemCount).Equals(1) && x.DepartmentFk != model.departmentFk).ToList();

                        foreach (var rating in ratingListUnique)
                        {
                            var fragment = duplicates2.Where(x => x.ProductFk.Equals(rating.ProductFk)).ToList();
                            int count = fragment.Count();
                            for (int i = 0; i < count; i++)
                            {
                                if (rating.Rating != 0)
                                {
                                    var value = rating.Rating - (float)(i * 0.1);
                                 //   var RatingChanged = dBContext.tb_rating.Single(x => x.rating_pk == rating.RatingPk);
                                  //  RatingChanged.rating_item = value;
                                }


                            }
                        }




                        dBContext.SaveChanges();

                        */


                        mb.UpdateRating(sendModel);


                        break;
                    }
                
                
                default: break;

            }

            orderHistory.status = HistoryStatus.Valid;
            return orderHistory;

        }

        public OrderHistoryModel FilterHistoryProducts(int user_id,FilterModel filter)
        {
            var orderhistory = new OrderHistoryModel();

            orderhistory.UserFk = user_id;
            orderhistory.itemList = dBContext.tb_history_item.Where(x => x.history_item_user_fk == user_id)
            .Select(x => new HistoryItemModel
            {
                HistoryItemPk = x.history_item_pk,
                ProductName = x.tb_product.product_name,
                ProductPrice = x.tb_product.product_price,
                ProductFk = x.history_item_product_fk,
                Rating = x.history_item_rating,
                UserFk = x.history_item_user_fk
            }).ToList();

            foreach (var item in orderhistory.itemList)
            {

                item.RatingStar = FillStarRating(item.HistoryItemPk, item.Rating);

                var image = dBContext.tb_product_images.SingleOrDefault(x => x.prod_id == item.ProductFk && x.prod_main_photo);
                item.slika = "data:image/jpg;base64," + System.Convert.ToBase64String(image.prod_image_data);
            }




           if (filter.Ten_to_20)
                orderhistory.itemList = orderhistory.itemList.Where(x => 10 <= x.ProductPrice && x.ProductPrice <= 20).ToList();

            if (filter.Twenty_to_30)
                orderhistory.itemList = orderhistory.itemList.Where(x => 20 <= x.ProductPrice && x.ProductPrice <= 30).ToList();

            if (filter.stars > 0)
            {
                foreach (var item in orderhistory.itemList)
                {
                   // int rating =  new Products(dBContext).GetRatingForProduct(item.ProductFk);
                    if (item.Rating != filter.stars)
                    {
                        orderhistory.itemList = orderhistory.itemList.Where(x => x.ProductFk != item.ProductFk).ToList();
                    }

                }
            }
          
            return orderhistory;
        }




        private string[] FillStarRating(int id, int rating)
        {
            var lista = new string[5];
            for(int i=0;i<5;i++)
            {
                if (i < rating)
                    lista[i] = "star_" + id;
                else
                    lista[i] = "star_border";
            }
            return lista;
        }
        
    }
}