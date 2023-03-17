﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Api.BusinessLogic.Common;
using Api.Models.myDatabase;
using Api.Models.myModels;
using Api.BusinessLogic;
using Api.BusinessLogic.Common;
using Microsoft.Ajax.Utilities;
using Api.Models.myEnums;
using ML.NET;

namespace Api.BusinessLogic
{
    public class Products : Base
    {

        public Products() : base(null) { }

        public Products(mydatabaseEntities _dbContext)
             : base(_dbContext)
        { }


        public List<ProductModel> GetProducts()
        {


            var images = dBContext.tb_product_images.Select(x => new ProductModel
            {
                ImageData = x.prod_image_data,
                ProductFk = x.prod_id,
                ProductImageName = "Slika",
                ProductPrice = x.tb_product.product_price,
                ProductPk = x.prod_image_pk
                
                

            }).ToList();
            foreach (var image in images)
            {
                //image.Image = imageHandler.byteArrayToImage(image.ImageData);
                image.slika = "data:image/jpg;base64," + System.Convert.ToBase64String(image.ImageData);
             }

            return images;

        }
        public List<ProductModel> GetGroupProducts()
        {
            var images = dBContext.tb_product.Select(x => new ProductModel
            {
                ProductName = x.product_name,
                ProductPrice = x.product_price,
                ProductPk = x.product_pk
            }).ToList();

            foreach (var image in images)
            {
                var slika = dBContext.tb_product_images.FirstOrDefault(x => x.prod_id == image.ProductPk && x.prod_main_photo);
                if (slika != null)
                    image.slika = "data:image/jpg;base64," + System.Convert.ToBase64String(slika.prod_image_data);
                else
                    image.slika = "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQEBLAEsAAD/4QBWRXhpZgAATU0AKgAAAAgABAEaAAUAAAABAAAAPgEbAAUAAAABAAAARgEoAAMAAAABAAIAAAITAAMAAAABAAEAAAAAAAAAAAEsAAAAAQAAASwAAAAB/+0ALFBob3Rvc2hvcCAzLjAAOEJJTQQEAAAAAAAPHAFaAAMbJUccAQAAAgAEAP/hDIFodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvADw/eHBhY2tldCBiZWdpbj0n77u/JyBpZD0nVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkJz8+Cjx4OnhtcG1ldGEgeG1sbnM6eD0nYWRvYmU6bnM6bWV0YS8nIHg6eG1wdGs9J0ltYWdlOjpFeGlmVG9vbCAxMS44OCc+CjxyZGY6UkRGIHhtbG5zOnJkZj0naHR0cDovL3d3dy53My5vcmcvMTk5OS8wMi8yMi1yZGYtc3ludGF4LW5zIyc+CgogPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9JycKICB4bWxuczp0aWZmPSdodHRwOi8vbnMuYWRvYmUuY29tL3RpZmYvMS4wLyc+CiAgPHRpZmY6UmVzb2x1dGlvblVuaXQ+MjwvdGlmZjpSZXNvbHV0aW9uVW5pdD4KICA8dGlmZjpYUmVzb2x1dGlvbj4zMDAvMTwvdGlmZjpYUmVzb2x1dGlvbj4KICA8dGlmZjpZUmVzb2x1dGlvbj4zMDAvMTwvdGlmZjpZUmVzb2x1dGlvbj4KIDwvcmRmOkRlc2NyaXB0aW9uPgoKIDxyZGY6RGVzY3JpcHRpb24gcmRmOmFib3V0PScnCiAgeG1sbnM6eG1wTU09J2h0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9tbS8nPgogIDx4bXBNTTpEb2N1bWVudElEPmFkb2JlOmRvY2lkOnN0b2NrOmRiMTUyNTRhLWMxZGUtNGEyNy1hODg1LTg4Y2E2MmJjMjA4YzwveG1wTU06RG9jdW1lbnRJRD4KICA8eG1wTU06SW5zdGFuY2VJRD54bXAuaWlkOjM3MzAyYzI0LWVjYjUtNGVmNi04MGYyLTIzN2MyZGZhNDVmYjwveG1wTU06SW5zdGFuY2VJRD4KIDwvcmRmOkRlc2NyaXB0aW9uPgo8L3JkZjpSREY+CjwveDp4bXBtZXRhPgogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAo8P3hwYWNrZXQgZW5kPSd3Jz8+/9sAQwAFAwQEBAMFBAQEBQUFBgcMCAcHBwcPCwsJDBEPEhIRDxERExYcFxMUGhURERghGBodHR8fHxMXIiQiHiQcHh8e/9sAQwEFBQUHBgcOCAgOHhQRFB4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4e/8AAEQgBaAHgAwERAAIRAQMRAf/EABwAAQACAwEBAQAAAAAAAAAAAAAHCAEFBgMCBP/EAFkQAAEDAwEDBgYNCQQFCwUAAAEAAgMEBQYRByExEiJBUXGBCBMUVmGRFhc3QlJydKGxs8HR0iMyMzQ2c5OUshUkYsI1gpKi8CU4Q1NUVWNldaPhREiGw8T/xAAUAQEAAAAAAAAAAAAAAAAAAAAA/8QAFBEBAAAAAAAAAAAAAAAAAAAAAP/aAAwDAQACEQMRAD8AtugwgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgyEBBhAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBBkICDCAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICDIQEGEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEGQgIMICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIMhAQYQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQZCAgwgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgyEBBhAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBBkICDCAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICDIQEGEBAQEBAQEBAQEBAQEBAQDroeSOUega6anqQVabtNzm35XLX1dyqHPjnc2a3zboAASDHyPe6cNRvHFBYjB8qteW2RlztkhGmjZ4HHnwP+C77DwIQb1AQEBAQEH47rdbZaohLdLjSULDwNRM1mvZqd6DTe2BhHnXaP46B7YGEeddp/joHtgYR512n+Oge2BhHnXaf46B7YGEeddp/joHtgYR512n+Oge2BhHnXaf46B7YGEeddp/joP32nJ8cu0nirZfbbVyHgyKpaXHu11Qbfp0QYQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEGQgIMICAgICAgICAgICAgICAgI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";
            }

            return images;
        }

        public ProductsPageModel GetProductsByCategoryFk(SearchModel model)
        {
            var consumeModel = new ConsumeProductModel();
            var predictions = new PredictionsModel();
            predictions.products = new List<ProductModel>();

            var ProductsPageModel = new ProductsPageModel();
            var productsIds = dBContext.tb_product.Where(x => x.product_category_fk.Equals(model.categoryFk)).Select(x => x.product_pk).ToList();
            foreach (var id in productsIds)
            {
                var result = consumeModel.Predict(
                   new ModelInput()
                   {
                       UserId = model.userId,
                       ProductId = id,
                   });
                if (result.valid && !float.IsNaN(result.output.Score))
                    predictions.products.Add(new ProductModel { ProductPk = id, Score = result.output.Score });
            }


            ProductsPageModel.products = dBContext.tb_product.Where(x => productsIds.Contains(x.product_pk)).Select(x => new ProductModel
            {
                ProductName = x.product_name,
                ProductPrice = x.product_price,
                ProductDetails = x.product_details,
                ProductPk = x.product_pk
            }).ToList();

            foreach (var item in ProductsPageModel.products)
            {
                item.Score = predictions.products.Any(x => x.ProductPk.Equals(item.ProductPk)) ?
                   predictions.products.Single(x => x.ProductPk.Equals(item.ProductPk)).Score
                   : float.NaN;

                item.Score = Singoid(item.Score);

                var slika = dBContext.tb_product_images.FirstOrDefault(x => x.prod_id == item.ProductPk && x.prod_main_photo);
                if (slika != null)
                    item.slika = "data:image/jpg;base64," + System.Convert.ToBase64String(slika.prod_image_data);
                else
                    item.slika = defaultPhoto;

                item.Rating = GetRatingForProduct(item.ProductPk);
                item.RatingStar = FillStarRating(item.Rating);

            }
            ProductsPageModel.products = ProductsPageModel.products.OrderByDescending(x => x.Score).ToList();
            return ProductsPageModel;
        }
        public List<ProductModel> GetProductListByCategoryFk(int categoryFk)
        {
            return dBContext.tb_product.Where(x => x.product_category_fk.Equals(categoryFk)).Select(x => new ProductModel
            {
                ProductName = x.product_name,
                ProductDetails = x.product_details,

                ProductPk = x.product_pk,
                ProductPrice = x.product_price
                
            }).ToList();
        }

        public ProductsPageModel GetProductsByDepartmentFk(SearchModel model)
        {
            var ProductsPageModel = new ProductsPageModel();

            var cat_ids = dBContext.tb_category.Where(x => x.category_department_fk == model.departmentFk).Select(x => x.category_pk).ToList();


            var consumeModel = new ConsumeProductModel();
            var predictions = new PredictionsModel();

            predictions.products = new List<ProductModel>();

            var productsIds = dBContext.tb_product.Where(x => cat_ids.Contains(x.product_category_fk)).Select(x => x.product_pk).ToList();
            foreach (var id in productsIds)
            {
                var result = consumeModel.Predict(
                   new ModelInput()
                   {
                       UserId = model.userId,
                       ProductId = id,
                   });
                if (result.valid && !float.IsNaN(result.output.Score))
                    predictions.products.Add(new ProductModel { ProductPk = id, Score = result.output.Score });
            }

            ProductsPageModel.products = dBContext.tb_product.Where(x => productsIds.Contains(x.product_pk)).Select(x => new ProductModel
            {
                ProductName = x.product_name,
                ProductDetails = x.product_details,
                ProductPrice = x.product_price,
                ProductPk = x.product_pk
            }).ToList();

            foreach (var item in ProductsPageModel.products)
            {
                item.Score = predictions.products.Any(x => x.ProductPk.Equals(item.ProductPk)) ?
                   predictions.products.Single(x => x.ProductPk.Equals(item.ProductPk)).Score
                   : float.NaN;

                item.Score = Singoid(item.Score);
                var slika = dBContext.tb_product_images.FirstOrDefault(x => x.prod_id == item.ProductPk && x.prod_main_photo);
                if (slika != null)
                    item.slika = "data:image/jpg;base64," + System.Convert.ToBase64String(slika.prod_image_data);
                else
                    item.slika = defaultPhoto;

                item.Rating = GetRatingForProduct(item.ProductPk);
                item.RatingStar = FillStarRating(item.Rating);
            }
            ProductsPageModel.products = ProductsPageModel.products.OrderByDescending(x => x.Score).ToList();

            return ProductsPageModel;
        }


        public ProductsPageModel FilterProducts(FilterModel filter)
        {
            var ProductsPageModel = new ProductsPageModel();
            var products = Enumerable.Empty<tb_product>().AsQueryable();
            switch (filter.status)
            {
                case FilterStatus.Category:
                    {
                        products = dBContext.tb_product.Where(x => x.product_category_fk.Equals(filter.categoryFk));
                        break;
                    }
                case FilterStatus.Department:
                    {
                        var cat_ids = dBContext.tb_category.Where(x => x.category_department_fk == filter.departmentFk).Select(x => x.category_pk).ToList();
                        products = dBContext.tb_product.Where(x => cat_ids.Contains(x.product_category_fk));
                        break;
                    }
                case FilterStatus.Search:
                    {
                        products = dBContext.tb_product.Where(x => x.product_name.Equals(filter.search_input));
                        break;
                    }
            }
          



            if (filter.Ten_to_20)
                products = products.Where(x => 10 <= x.product_price && x.product_price <= 20);

            if (filter.Twenty_to_30)
                products = products.Where(x => 20 <= x.product_price && x.product_price <= 30);

            if (filter.stars > 0)
            {
                foreach(var item in products.ToList())
                {
                    int rating = GetRatingForProduct(item.product_pk);
                        if (rating != filter.stars)
                        {
                            products = products.Where(x => x.product_pk != item.product_pk);
                        }
                    
                }
            }
               // products = products.Where(x => x.tb_rating.);


            ProductsPageModel.products = products
             .Select(x => new ProductModel
                {
                    ProductName = x.product_name,
                     ProductDetails = x.product_details,
                     ProductPrice = x.product_price,
                    ProductPk = x.product_pk
                }).ToList();

            foreach (var item in ProductsPageModel.products)
            {
                var slika = dBContext.tb_product_images.FirstOrDefault(x => x.prod_id == item.ProductPk && x.prod_main_photo);
                if (slika != null)
                    item.slika = "data:image/jpg;base64," + System.Convert.ToBase64String(slika.prod_image_data);
                else
                    item.slika = defaultPhoto;

                item.Rating = GetRatingForProduct(item.ProductPk);
                item.RatingStar = FillStarRating(item.Rating);
            }
            return ProductsPageModel;
        }
        public List<ProductModel> GetProductbycode(int id)
        {            

          
            var images = dBContext.tb_product_images.Where(x=>x.prod_id.Equals(id)).Select(x => new ProductModel
            {
                ImageData = x.prod_image_data,
                ProductPk = x.prod_id,
                ProductImageName = "Slika",
                temp = x.prod_image_pk,
                ProductPrice = x.tb_product.product_price

            }).ToList();

          
          

            foreach (var image in images)
            {
                image.ProductPk = image.temp;
                image.slika = "data:image/jpg;base64," + System.Convert.ToBase64String(image.ImageData);
            }

            return images;

        }
        public ProductModel GetProductbyId(int id)
        {
            var product = dBContext.tb_product.Where(x=>x.product_pk == id).Select(x => new ProductModel
            {
                ProductPk = x.product_pk,
                ProductName = x.product_name,
                ProductDetails = x.product_details,
                ProductPrice = x.product_price

            }).SingleOrDefault();

            product.Images = dBContext.tb_product_images.Where(x => x.prod_id == id).Select(x => new ImageModel
            {     
                RecordPk = x.prod_image_pk,
                MainPhoto = x.prod_main_photo,
                RecordFk = x.prod_id,
                ImageData = x.prod_image_data,
            }).ToList();
            product.current_image = product.Images.Single(x => x.MainPhoto);

            var mainPhoto = product.Images.Single(x => x.MainPhoto);
            product.slika = "data:image/jpg;base64," + System.Convert.ToBase64String(mainPhoto.ImageData);

            foreach (var slika in product.Images)
            {
                if (slika != null)
                {
                   
                    slika.src = "data:image/jpg;base64," + System.Convert.ToBase64String(slika.ImageData);
                }
                else
                    slika.src = "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQEBLAEsAAD/4QBWRXhpZgAATU0AKgAAAAgABAEaAAUAAAABAAAAPgEbAAUAAAABAAAARgEoAAMAAAABAAIAAAITAAMAAAABAAEAAAAAAAAAAAEsAAAAAQAAASwAAAAB/+0ALFBob3Rvc2hvcCAzLjAAOEJJTQQEAAAAAAAPHAFaAAMbJUccAQAAAgAEAP/hDIFodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvADw/eHBhY2tldCBiZWdpbj0n77u/JyBpZD0nVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkJz8+Cjx4OnhtcG1ldGEgeG1sbnM6eD0nYWRvYmU6bnM6bWV0YS8nIHg6eG1wdGs9J0ltYWdlOjpFeGlmVG9vbCAxMS44OCc+CjxyZGY6UkRGIHhtbG5zOnJkZj0naHR0cDovL3d3dy53My5vcmcvMTk5OS8wMi8yMi1yZGYtc3ludGF4LW5zIyc+CgogPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9JycKICB4bWxuczp0aWZmPSdodHRwOi8vbnMuYWRvYmUuY29tL3RpZmYvMS4wLyc+CiAgPHRpZmY6UmVzb2x1dGlvblVuaXQ+MjwvdGlmZjpSZXNvbHV0aW9uVW5pdD4KICA8dGlmZjpYUmVzb2x1dGlvbj4zMDAvMTwvdGlmZjpYUmVzb2x1dGlvbj4KICA8dGlmZjpZUmVzb2x1dGlvbj4zMDAvMTwvdGlmZjpZUmVzb2x1dGlvbj4KIDwvcmRmOkRlc2NyaXB0aW9uPgoKIDxyZGY6RGVzY3JpcHRpb24gcmRmOmFib3V0PScnCiAgeG1sbnM6eG1wTU09J2h0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9tbS8nPgogIDx4bXBNTTpEb2N1bWVudElEPmFkb2JlOmRvY2lkOnN0b2NrOmRiMTUyNTRhLWMxZGUtNGEyNy1hODg1LTg4Y2E2MmJjMjA4YzwveG1wTU06RG9jdW1lbnRJRD4KICA8eG1wTU06SW5zdGFuY2VJRD54bXAuaWlkOjM3MzAyYzI0LWVjYjUtNGVmNi04MGYyLTIzN2MyZGZhNDVmYjwveG1wTU06SW5zdGFuY2VJRD4KIDwvcmRmOkRlc2NyaXB0aW9uPgo8L3JkZjpSREY+CjwveDp4bXBtZXRhPgogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAo8P3hwYWNrZXQgZW5kPSd3Jz8+/9sAQwAFAwQEBAMFBAQEBQUFBgcMCAcHBwcPCwsJDBEPEhIRDxERExYcFxMUGhURERghGBodHR8fHxMXIiQiHiQcHh8e/9sAQwEFBQUHBgcOCAgOHhQRFB4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4e/8AAEQgBaAHgAwERAAIRAQMRAf/EABwAAQACAwEBAQAAAAAAAAAAAAAHCAEFBgMCBP/EAFkQAAEDAwEDBgYNCQQFCwUAAAEAAgMEBQYRByExEiJBUXGBCBMUVmGRFhc3QlJydKGxs8HR0iMyMzQ2c5OUshUkYsI1gpKi8CU4Q1NUVWNldaPhREiGw8T/xAAUAQEAAAAAAAAAAAAAAAAAAAAA/8QAFBEBAAAAAAAAAAAAAAAAAAAAAP/aAAwDAQACEQMRAD8AtugwgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgyEBBhAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBBkICDCAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICDIQEGEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEGQgIMICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIMhAQYQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQZCAgwgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgyEBBhAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBBkICDCAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICDIQEGEBAQEBAQEBAQEBAQEBAQDroeSOUega6anqQVabtNzm35XLX1dyqHPjnc2a3zboAASDHyPe6cNRvHFBYjB8qteW2RlztkhGmjZ4HHnwP+C77DwIQb1AQEBAQEH47rdbZaohLdLjSULDwNRM1mvZqd6DTe2BhHnXaP46B7YGEeddp/joHtgYR512n+Oge2BhHnXaf46B7YGEeddp/joHtgYR512n+Oge2BhHnXaf46B7YGEeddp/joP32nJ8cu0nirZfbbVyHgyKpaXHu11Qbfp0QYQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEGQgIMICAgICAgICAgICAgICAgI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";

            }

           

          
            product.Rating = GetRatingForProduct(id);
            product.RatingStar = FillStarRating(product.Rating);
            

            return product;

        }
      

        public ProductsPageModel SearchProducts(SearchModel model)
        {
            var consumeModel = new ConsumeProductModel();
            var predictions = new PredictionsModel();

            predictions.products = new List<ProductModel>();

            var productsIds = dBContext.tb_product.Where(x => x.product_name.Contains(model.search)).Select(x => x.product_pk).ToList();
            foreach (var id in productsIds)
            {
                var result = consumeModel.Predict(
                   new ModelInput()
                   {
                       UserId = model.userId,
                       ProductId = id,
                   });
                if (result.valid && !float.IsNaN(result.output.Score))
                    predictions.products.Add(new ProductModel { ProductPk = id, Score = result.output.Score });
            }

            var ProductsPageModel = new ProductsPageModel();

            var products = dBContext.tb_product.Where(x=> productsIds.Contains(x.product_pk)).Select(x => new ProductModel
            {
                ProductName = x.product_name,
                ProductDetails = x.product_details,
                ProductPrice = x.product_price,
                ProductPk = x.product_pk,

        }).ToList();

            foreach (var item in products)
            {
                item.Score = predictions.products.Any(x => x.ProductPk.Equals(item.ProductPk)) ?
                    predictions.products.Single(x => x.ProductPk.Equals(item.ProductPk)).Score 
                    : float.NaN;
                item.Score = Singoid(item.Score);

                var slika = dBContext.tb_product_images.FirstOrDefault(x => x.prod_id == item.ProductPk && x.prod_main_photo);
                if (slika != null)
                    item.slika = "data:image/jpg;base64," + System.Convert.ToBase64String(slika.prod_image_data);
                else
                    item.slika = defaultPhoto;

                item.Rating = GetRatingForProduct(item.ProductPk);
                item.RatingStar = FillStarRating(item.Rating);
            }

            ProductsPageModel.products = products.OrderByDescending(x=>x.Score).ToList();
            return ProductsPageModel;
        }
        public int GetRatingForProduct(int produktId)
        {
            var rating = dBContext.tb_rating.Where(x => x.rating_product_fk == produktId).Select(x => new RatingModel
            {
                UserFk = x.rating_user_fk,
                Rating =  (float) Math.Ceiling(x.rating_item),

            }).ToList();

            var district = rating.DistinctBy(x => x.UserFk);
            var ratings = district.Select(x => x.Rating).ToList();
            var UserCount = rating.Select(x => x.UserFk).ToList().Count();
            if (UserCount >= 1)
                return CalculateRating(ratings, UserCount);    
            else return 0;
        }
        public SharedModel SetProduct(ProductUpload product)
        {
            var returnModel = new SharedModel();

            returnModel.errors = new List<string>();

            if (product.productStatus == Models.myEnums.ProductStatus.CreateProduct)
            {

                var addProduct = new tb_product
                {
                    product_name = product.ProductName,
                    product_details = product.ProductDetails,
                    product_price = product.ProductPrice,
                    product_category_fk = product.CategoryFk,
                    start_date = DateTime.Now,
                    createdby = "Igor",
                    createdondate = DateTime.Now

                };
                dBContext.tb_product.Add(addProduct);

                dBContext.SaveChanges();


                string output = product.Base64File.Substring(product.Base64File.IndexOf(',') + 1);
                var addImage = new tb_product_images
                {
                    prod_id = addProduct.product_pk,
                    prod_image_data = Convert.FromBase64String(output),
                    prod_main_photo = true,
                    start_date = DateTime.Now,
                    createdby = "Igor",
                    createdondate = DateTime.Now

                };

                dBContext.tb_product_images.Add(addImage);
                dBContext.SaveChanges();
            }
            else
            {
                var update = dBContext.tb_product.Single(x => x.product_pk == product.ProductPk);
                update.lastupdatedby = "admin";
                update.lastupdateddate = DateTime.Now;
                update.product_name = product.ProductName;
                update.product_details = product.ProductDetails;
                update.product_price = product.ProductPrice;

                

                if (!string.IsNullOrEmpty(product.Base64File))
                {                
                    
                    string output = product.Base64File.Substring(product.Base64File.IndexOf(',') + 1);


                    if (!product.MainPhoto)
                    {
                        var addImage = new tb_product_images
                        {

                            prod_id = update.product_pk,
                            prod_image_data = Convert.FromBase64String(output),
                            prod_main_photo = product.MainPhoto,
                            start_date = DateTime.Now,
                            createdby = "Igor",
                            createdondate = DateTime.Now,

                        };
                        dBContext.tb_product_images.Add(addImage);
                    }
                    else
                    {
                        var oldMainphoto = dBContext.tb_product_images.Single(x => x.prod_id == product.ProductPk && x.prod_main_photo);
                        oldMainphoto.prod_main_photo = false;
                        oldMainphoto.lastupdatedby = "admin";
                        oldMainphoto.lastupdateddate = DateTime.Now;

                        var addImage = new tb_product_images
                        {

                            prod_id = update.product_pk,
                            prod_image_data = Convert.FromBase64String(output),
                            prod_main_photo = true,
                            start_date = DateTime.Now,
                            createdby = "Igor",
                            createdondate = DateTime.Now,

                        };
                        dBContext.tb_product_images.Add(addImage);
                    }

                }
               
                
                dBContext.SaveChanges();

            }

            returnModel.Status = Models.myEnums.CreateStatus.sucess;
            return returnModel;
        }
     }

  }
