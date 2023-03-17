using Api.Models.myDatabase;
using Api.Models.myModels;
using ML.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.BusinessLogic
{
    public class Home : Base
    {
        public Home() : base(null) {
        }

        public Home(mydatabaseEntities _dbContext)
             : base(_dbContext)
        {
        }

        public HomeModel GetHome(int user_id)
        {
            var home = new HomeModel();
            var predictions = new PredictionsModel();
            var rand = new Random();


            #region Predict Products          

            var consumeModel = new ConsumeProductModel();

            predictions.products = new List<ProductModel>();


            var productsIds = dBContext.tb_product.Select(x => x.product_pk).ToList();
            foreach (var id in productsIds)
            {
                var result = consumeModel.Predict(
                   new ModelInput()
                   {
                       UserId = user_id,
                       ProductId = id,
                   });
                if (result.valid && !float.IsNaN(result.output.Score))
                    predictions.products.Add(new ProductModel { ProductPk = id, Score = result.output.Score });
            }

            // predictions.products = predictions.products.OrderByDescending(x => x.Score).ToList().Take(4).ToList();

            #endregion


            #region Predict Departments
            var ConsumeDepartmentModel = new ConsumeDepartmentModel();

            predictions.departments = new List<DepartmentModel>();

            var departmentIds = dBContext.tb_department.Select(x => x.department_pk).ToList();
            foreach (var id in departmentIds)
            {
                var result = ConsumeDepartmentModel.PredictDatabase(
                   new DepartmentModelInput()
                   {
                       UserId = user_id,
                       DepartmentId = id,
                   });
                if (result.valid && !float.IsNaN(result.output.Score))
                    predictions.departments.Add(new DepartmentModel { RecordPk = id, Score = result.output.Score });
            }

            //predictions.departments = predictions.departments.OrderByDescending(x => x.Score).ToList().Take(4).ToList();

            #endregion


            #region Predict Category
            var ConsumeCategoryModel = new ConsumeCategoryModel();

            predictions.categories = new List<CategoryModel>();

            var categoryIds = dBContext.tb_category.Select(x => x.category_pk).ToList();
            foreach (var id in categoryIds)
            {
                var result = ConsumeCategoryModel.PredictDatabase(
                   new CategoryModelInput()
                   {
                       UserId = user_id,
                       CategoryId = id,
                   });
                if (result.valid && !float.IsNaN(result.output.Score))
                    predictions.categories.Add(new CategoryModel { RecordPk = id, Score = result.output.Score });
            }

            //predictions.departments = predictions.departments.OrderByDescending(x => x.Score).ToList().Take(4).ToList();

            #endregion


            #region OcitajDepartmante


            var predictedDepartmants = new List<int>();

            if (predictions.departments.Count() > 0)
            {
                if (predictions.departments.Count() < 4)
                {
                    int count = predictions.departments.Count();

                    predictedDepartmants = predictions.departments.OrderByDescending(x => x.Score).Select(x => x.RecordPk).ToList();
                    predictedDepartmants.AddRange(departmentIds.Except(predictedDepartmants).OrderBy(x => rand.Next()).Take(4 - count));
                }
                else
                    predictedDepartmants = predictions.departments.OrderByDescending(x => x.Score).Take(4).Select(x => x.RecordPk).ToList();
            }
            else
                predictedDepartmants = departmentIds.OrderBy(x => rand.Next()).Take(4).ToList();

            // predictedDepartmants = predictions.departments.Count() > 0 ? predictions.departments.OrderByDescending(x => x.Score).Take(4).Select(x => x.RecordPk).ToList()
            //  : departmentIds.OrderBy(x => rand.Next()).Take(4);

            home.departmentList = dBContext.tb_department.Where(x => predictedDepartmants.Contains(x.department_pk)).Select(x => new DepartmentModel
            {
                RecordPk = x.department_pk,
                DepartmentName = x.department_name,


            }).ToList();

            foreach (var item in home.departmentList)
            {
                item.Score = predictions.departments.Any(x => x.RecordPk.Equals(item.RecordPk)) ? predictions.departments.Single(x => x.RecordPk.Equals(item.RecordPk)).Score : float.NaN;

                var bytes = dBContext.tb_dept_cat_images.SingleOrDefault(x => x.dept_cat_department_fk.Value.Equals(item.RecordPk)).dept_cat_image_data;
                if (bytes != null)
                {
                    item.ImageData = bytes;
                    item.slika = "data:image/jpg;base64," + System.Convert.ToBase64String(item.ImageData);
                }
                else
                    item.slika = defaultPhoto;
            }

            home.departmentList = home.departmentList.OrderByDescending(x => x.Score).ToList();
            #endregion OcitajDepartmante


            #region OcitajCategory

            var predictedCategory = predictions.categories.Count() > 0 ? predictions.categories.OrderByDescending(x => x.Score).Take(4).Select(x => x.RecordPk).ToList()
               : categoryIds.OrderBy(x => rand.Next()).Take(4);

            home.categoryList = dBContext.tb_category.Where(x => predictedCategory.Contains(x.category_pk)).Select(x => new CategoryModel
            {
                RecordPk = x.category_pk,
                CategoryName = x.category_name,
                DepartmentFk = x.category_department_fk


            }).Take(4).ToList();

            home.categoryDepartmentFk = home.categoryList[0].DepartmentFk;

            foreach (var item in home.categoryList)
            {
                item.Score = predictions.categories.Count() > 0 ? predictions.categories.Single(x => x.RecordPk.Equals(item.RecordPk)).Score : float.NaN;

                var bytes = dBContext.tb_dept_cat_images.SingleOrDefault(x => x.dept_cat_category_fk.Value.Equals(item.RecordPk)).dept_cat_image_data;
                if (bytes != null)
                {
                    item.ImageData = bytes;
                    item.slika = "data:image/jpg;base64," + System.Convert.ToBase64String(item.ImageData);
                }
                else
                    item.slika = defaultPhoto;
            }

            home.categoryList = home.categoryList.OrderByDescending(x => x.Score).ToList();
            #endregion OcitajCategory



            #region Ocitaj Proizvode

            var predictedProducts = predictions.products.Count() > 0 ? predictions.products.OrderByDescending(x => x.Score).Take(4).Select(x => x.ProductPk).ToList()
                : productsIds.OrderBy(x => rand.Next()).Take(4);

            home.productsRow1_predictions1 = dBContext.tb_product.Where(x => predictedProducts.Contains(x.product_pk)).Select(x => new ProductModel
            {
                ProductName = x.product_name,
                ProductPrice = x.product_price,
                ProductPk = x.product_pk
            }).ToList();

            foreach (var item in home.productsRow1_predictions1)
            {


                item.Score = predictions.products.Count() > 0 ? predictions.products.Single(x => x.ProductPk.Equals(item.ProductPk)).Score : float.NaN;
                var slika = dBContext.tb_product_images.FirstOrDefault(x => x.prod_id == item.ProductPk && x.prod_main_photo);
                if (slika != null)
                    item.slika = "data:image/jpg;base64," + System.Convert.ToBase64String(slika.prod_image_data);
                else
                    item.slika = defaultPhoto;


            }
            home.productsRow1_predictions1 = home.productsRow1_predictions1.OrderByDescending(x => x.Score).ToList();

            var ids = home.productsRow1_predictions1.Select(x => x.ProductPk).ToList();

            home.productsRow1_predictions2 = dBContext.tb_product.Where(x => !ids.Contains(x.product_pk)).Select(x => new ProductModel
            {
                ProductName = x.product_name,
                ProductPrice = x.product_price,
                ProductPk = x.product_pk
            }).Take(4).ToList();


            foreach (var item in home.productsRow1_predictions2)
            {


                var slika = dBContext.tb_product_images.FirstOrDefault(x => x.prod_id == item.ProductPk && x.prod_main_photo);
                if (slika != null)
                    item.slika = "data:image/jpg;base64," + System.Convert.ToBase64String(slika.prod_image_data);
                else
                    item.slika = defaultPhoto;



            }

            home.productsRow2_predictions = home.productsRow1_predictions2;


            #endregion Ocitaj Proizvode



            return home;

        }

        //Recomanded products
        public List<ProductModel> GetRow1Products_1(int user_id)
        {
            var predictions = new PredictionsModel();
            var rand = new Random();


            #region Predict Products          

            var consumeModel = new ConsumeProductModel();

            predictions.products = new List<ProductModel>();

            var historyProductsIds = dBContext.tb_history_item.Where(x => x.history_item_user_fk == user_id).Select(x => x.history_item_product_fk).ToList();
            var productsIds = dBContext.tb_product.Where(x => !historyProductsIds.Contains(x.product_pk)).Select(x => x.product_pk).ToList();
            foreach (var id in productsIds)
            {
                var result = consumeModel.Predict(
                   new ModelInput()
                   {
                       UserId = user_id,
                       ProductId = id,
                   });
                if (result.valid && !float.IsNaN(result.output.Score))
                    predictions.products.Add(new ProductModel { ProductPk = id, Score = result.output.Score });
            }


            #endregion



            #region Ocitaj Proizvode


            var predictedProducts = new List<int>();

            if (predictions.products.Count() > 0)
            {
                if (predictions.products.Count() < 4)
                {
                    int count = predictions.products.Count();

                    predictedProducts = predictions.products.OrderByDescending(x => x.Score).Select(x => x.ProductPk).ToList();
                    predictedProducts.AddRange(productsIds.Except(predictedProducts).OrderBy(x => rand.Next()).Take(4 - count));
                }
                else
                    predictedProducts = predictions.products.OrderByDescending(x => x.Score).Take(4).Select(x => x.ProductPk).ToList();
            }
            else
                predictedProducts = productsIds.OrderBy(x => rand.Next()).Take(4).ToList();





            var productsRow1_predictions1 = dBContext.tb_product.Where(x => predictedProducts.Contains(x.product_pk)).Select(x => new ProductModel
            {
                ProductName = x.product_name,
                ProductPrice = x.product_price,
                ProductPk = x.product_pk
            }).ToList();

            foreach (var item in productsRow1_predictions1)
            {
                

                item.Score = predictions.products.Any(x => x.ProductPk.Equals(item.ProductPk)) ? predictions.products.Single(x => x.ProductPk.Equals(item.ProductPk)).Score : float.NaN;
                item.Score = Singoid(item.Score);
                var slika = dBContext.tb_product_images.FirstOrDefault(x => x.prod_id == item.ProductPk && x.prod_main_photo);
                if (slika != null)
                    item.slika = "data:image/jpg;base64," + System.Convert.ToBase64String(slika.prod_image_data);
                else
                    item.slika = defaultPhoto;


            }
            productsRow1_predictions1 = productsRow1_predictions1.OrderByDescending(x => x.Score).ToList();


            #endregion Ocitaj Proizvode

            return productsRow1_predictions1;
        }
        //Keep shoping for products
        public List<ProductModel> GetRow1Products_2(int user_id)
        {
            var predictions = new PredictionsModel();
            var rand = new Random();


            #region Predict Products          

            var consumeModel = new ConsumeProductModel();

            predictions.products = new List<ProductModel>();

            var historyProductsIds = dBContext.tb_history_item.Where(x => x.history_item_user_fk == user_id).Select(x => x.history_item_product_fk).ToList();

            var productsIds = dBContext.tb_product.Select(x => x.product_pk).ToList();
            foreach (var id in historyProductsIds)
            {
                var result = consumeModel.Predict(
                   new ModelInput()
                   {
                       UserId = user_id,
                       ProductId = id,
                   });
                if (result.valid && !float.IsNaN(result.output.Score))
                    predictions.products.Add(new ProductModel { ProductPk = id, Score = result.output.Score });
            }


            #endregion



            #region Ocitaj Proizvode


            var predictedProducts = new List<int>();

            if (predictions.products.Count() > 0)
            {
                if (predictions.products.Count() < 4)
                {
                    int count = predictions.products.Count();

                    predictedProducts = predictions.products.OrderByDescending(x => x.Score).Select(x => x.ProductPk).ToList();
                    predictedProducts.AddRange(productsIds.Except(predictedProducts).OrderBy(x => rand.Next()).Take(4 - count));
                }
                else
                    predictedProducts = predictions.products.OrderByDescending(x => x.Score).Take(4).Select(x => x.ProductPk).ToList();
            }
            else
                predictedProducts = productsIds.OrderBy(x => rand.Next()).Take(4).ToList();





            var productsRow1_predictions2 = dBContext.tb_product.Where(x => predictedProducts.Contains(x.product_pk)).Select(x => new ProductModel
            {
                ProductName = x.product_name,
                ProductPrice = x.product_price,
                ProductPk = x.product_pk
            }).ToList();

            foreach (var item in productsRow1_predictions2)
            {


                item.Score = predictions.products.Any(x => x.ProductPk.Equals(item.ProductPk)) ? predictions.products.Single(x => x.ProductPk.Equals(item.ProductPk)).Score : float.NaN;
                item.Score = Singoid(item.Score);

                var slika = dBContext.tb_product_images.FirstOrDefault(x => x.prod_id == item.ProductPk && x.prod_main_photo);
                if (slika != null)
                    item.slika = "data:image/jpg;base64," + System.Convert.ToBase64String(slika.prod_image_data);
                else
                    item.slika = defaultPhoto;


            }
            productsRow1_predictions2 = productsRow1_predictions2.OrderByDescending(x => x.Score).ToList();


            #endregion Ocitaj Proizvode

            return productsRow1_predictions2;
        }

        public List<DepartmentModel> GetDepartments(int user_id)
        {
            var rand = new Random();
            var predictions = new PredictionsModel();

            #region Predict Departments
            var ConsumeDepartmentModel = new ConsumeDepartmentModel();

            predictions.departments = new List<DepartmentModel>();

            var departmentIds = dBContext.tb_department.Select(x => x.department_pk).ToList();
            foreach (var id in departmentIds)
            {
                var result = ConsumeDepartmentModel.PredictDatabase(
                   new DepartmentModelInput()
                   {
                       UserId = user_id,
                       DepartmentId = id,
                   });
                if (result.valid && !float.IsNaN(result.output.Score))
                    predictions.departments.Add(new DepartmentModel { RecordPk = id, Score = result.output.Score });
            }


            #endregion


            #region OcitajDepartmante


            var predictedDepartmants = new List<int>();

            if (predictions.departments.Count() > 0)
            {
                if (predictions.departments.Count() < 4)
                {
                    int count = predictions.departments.Count();

                    predictedDepartmants = predictions.departments.OrderByDescending(x => x.Score).Select(x => x.RecordPk).ToList();
                    predictedDepartmants.AddRange(departmentIds.Except(predictedDepartmants).OrderBy(x => rand.Next()).Take(4 - count));
                }
                else
                    predictedDepartmants = predictions.departments.OrderByDescending(x => x.Score).Take(4).Select(x => x.RecordPk).ToList();
            }
            else
                predictedDepartmants = departmentIds.OrderBy(x => rand.Next()).Take(4).ToList();

            // predictedDepartmants = predictions.departments.Count() > 0 ? predictions.departments.OrderByDescending(x => x.Score).Take(4).Select(x => x.RecordPk).ToList()
            //  : departmentIds.OrderBy(x => rand.Next()).Take(4);

            var departmentList = dBContext.tb_department.Where(x => predictedDepartmants.Contains(x.department_pk)).Select(x => new DepartmentModel
            {
                RecordPk = x.department_pk,
                DepartmentName = x.department_name,


            }).ToList();

            foreach (var item in departmentList)
            {
                item.Score = predictions.departments.Any(x => x.RecordPk.Equals(item.RecordPk)) ? predictions.departments.Single(x => x.RecordPk.Equals(item.RecordPk)).Score : float.NaN;
                item.Score = Singoid(item.Score);

                var bytes = dBContext.tb_dept_cat_images.SingleOrDefault(x => x.dept_cat_department_fk.Value.Equals(item.RecordPk)).dept_cat_image_data;
                if (bytes != null)
                {
                    item.ImageData = bytes;
                    item.slika = "data:image/jpg;base64," + System.Convert.ToBase64String(item.ImageData);
                }
                else
                    item.slika = defaultPhoto;
            }

            departmentList = departmentList.OrderByDescending(x => x.Score).ToList();
            #endregion OcitajDepartmante

            return departmentList;
        }

        public List<CategoryModel> GetCategories(int user_id)
        {
            var predictions = new PredictionsModel();
            var rand = new Random();

            #region Predict Category
            var ConsumeCategoryModel = new ConsumeCategoryModel();

            predictions.categories = new List<CategoryModel>();


            var categoryIds = dBContext.tb_category.Select(x => x.category_pk).ToList();
            foreach (var id in categoryIds)
            {
                var result = ConsumeCategoryModel.PredictDatabase(
                   new CategoryModelInput()
                   {
                       UserId = user_id,
                       CategoryId = id,
                   });
                if (result.valid && !float.IsNaN(result.output.Score))
                    predictions.categories.Add(new CategoryModel { RecordPk = id, Score = result.output.Score });
            }

            //predictions.departments = predictions.departments.OrderByDescending(x => x.Score).ToList().Take(4).ToList();

            #endregion


            #region OcitajCategory

            var predictedCategories = new List<int>();

            if (predictions.categories.Count() > 0)
            {
                if (predictions.categories.Count() < 4)
                {
                    int count = predictions.categories.Count();

                    predictedCategories = predictions.categories.OrderByDescending(x => x.Score).Select(x => x.RecordPk).ToList();
                    predictedCategories.AddRange(categoryIds.Except(predictedCategories).OrderBy(x => rand.Next()).Take(4 - count));
                }
                else
                    predictedCategories = predictions.categories.OrderByDescending(x => x.Score).Take(4).Select(x => x.RecordPk).ToList();
            }
            else
                predictedCategories = categoryIds.OrderBy(x => rand.Next()).Take(4).ToList();


            var categoryList = dBContext.tb_category.Where(x => predictedCategories.Contains(x.category_pk)).Select(x => new CategoryModel
            {
                RecordPk = x.category_pk,
                CategoryName = x.category_name,
                DepartmentFk = x.category_department_fk


            }).Take(4).ToList();

            //  home.categoryDepartmentFk = home.categoryList[0].DepartmentFk;

            foreach (var item in categoryList)
            {
                item.Score = predictions.categories.Any(x => x.RecordPk.Equals(item.RecordPk)) ? predictions.categories.Single(x => x.RecordPk.Equals(item.RecordPk)).Score : float.NaN;
                item.Score = Singoid(item.Score);

                var bytes = dBContext.tb_dept_cat_images.SingleOrDefault(x => x.dept_cat_category_fk.Value.Equals(item.RecordPk)).dept_cat_image_data;
                if (bytes != null)
                {
                    item.ImageData = bytes;
                    item.slika = "data:image/jpg;base64," + System.Convert.ToBase64String(item.ImageData);
                }
                else
                    item.slika = defaultPhoto;
            }

            categoryList = categoryList.OrderByDescending(x => x.Score).ToList();
            #endregion OcitajCategory

            return categoryList;

        }


        private  List<CategoryModel> GetPrivateCategories(int user_id)
        {
            var predictions = new PredictionsModel();
            var rand = new Random();

            #region Predict Category
            var ConsumeCategoryModel = new ConsumeCategoryModel();

            predictions.categories = new List<CategoryModel>();

            var productIds = dBContext.tb_history_item.Where(x => x.history_item_user_fk == user_id).Select(x => x.history_item_product_fk).Distinct().ToList();
            var categoryIds = dBContext.tb_product.Where(x => productIds.Contains(x.product_pk)).Select(x => x.product_category_fk).ToList();
           
            #endregion


            #region OcitajCategory

            var predictedCategories = new List<int>();

            if (predictions.categories.Count() > 0)
            {
                if (predictions.categories.Count() < 4)
                {
                    int count = predictions.categories.Count();

                    predictedCategories = predictions.categories.OrderByDescending(x => x.Score).Select(x => x.RecordPk).ToList();
                    predictedCategories.AddRange(categoryIds.Except(predictedCategories).OrderBy(x => rand.Next()).Take(4 - count));
                }
                else
                    predictedCategories = predictions.categories.OrderByDescending(x => x.Score).Take(4).Select(x => x.RecordPk).ToList();
            }
            else
                predictedCategories = categoryIds.OrderBy(x => rand.Next()).Take(4).ToList();


            var categoryList = dBContext.tb_category.Where(x => predictedCategories.Contains(x.category_pk)).Select(x => new CategoryModel
            {
                RecordPk = x.category_pk,
                CategoryName = x.category_name,
                DepartmentFk = x.category_department_fk


            }).Take(4).ToList();

            //  home.categoryDepartmentFk = home.categoryList[0].DepartmentFk;

            foreach (var item in categoryList)
            {
                item.Score = predictions.categories.Any(x => x.RecordPk.Equals(item.RecordPk)) ? predictions.categories.Single(x => x.RecordPk.Equals(item.RecordPk)).Score : float.NaN;
                item.Score = Singoid(item.Score);

                var bytes = dBContext.tb_dept_cat_images.SingleOrDefault(x => x.dept_cat_category_fk.Value.Equals(item.RecordPk)).dept_cat_image_data;
                if (bytes != null)
                {
                    item.ImageData = bytes;
                    item.slika = "data:image/jpg;base64," + System.Convert.ToBase64String(item.ImageData);
                }
                else
                    item.slika = defaultPhoto;
            }

            categoryList = categoryList.OrderByDescending(x => x.Score).ToList();
            #endregion OcitajCategory

            return categoryList;

        }

        public HomeRow2Model GetRow2(int user_id)
        {

            var homeRow2 = new HomeRow2Model();

            var rand = new Random();


            #region Predict Departments
            var ConsumeDepartmentModel = new ConsumeDepartmentModel();

            var departments = new List<DepartmentModel>();

            var departmentIds = dBContext.tb_department.Select(x => x.department_pk).ToList();
            foreach (var id in departmentIds)
            {
                var result = ConsumeDepartmentModel.PredictDatabase(
                   new DepartmentModelInput()
                   {
                       UserId = user_id,
                       DepartmentId = id,
                   });
                if (result.valid && !float.IsNaN(result.output.Score))
                    departments.Add(new DepartmentModel { RecordPk = id, Score = result.output.Score });
            }

            departments = departments.OrderByDescending(x => x.Score).Take(1).ToList();

            #endregion

            #region Predict Category
            var ConsumeCategoryModel = new ConsumeCategoryModel();

            var categories = new List<CategoryModel>();

            var categoryIds = dBContext.tb_category.Select(x => x.category_pk).ToList();
            foreach (var id in categoryIds)
            {
                var result = ConsumeCategoryModel.PredictDatabase(
                   new CategoryModelInput()
                   {
                       UserId = user_id,
                       CategoryId = id,
                   });
                if (result.valid && !float.IsNaN(result.output.Score))
                    categories.Add(new CategoryModel { RecordPk = id, Score = result.output.Score });
            }

            categories = categories.OrderByDescending(x => x.Score).Take(1).ToList();

            #endregion

            #region Predict Products          

            var consumeModel = new ConsumeProductModel();

            var products = new List<ProductModel>();

            var historyProductsIds = dBContext.tb_history_item.Where(x => x.history_item_user_fk == user_id).Select(x => x.history_item_product_fk).ToList();

            var productsIds = dBContext.tb_product.Where(x=> !historyProductsIds.Contains(x.product_pk)).Select(x => x.product_pk).ToList();
            foreach (var id in productsIds)
            {
                var result = consumeModel.Predict(
                   new ModelInput()
                   {
                       UserId = user_id,
                       ProductId = id,
                   });
                if (result.valid && !float.IsNaN(result.output.Score))
                    products.Add(new ProductModel { ProductPk = id, Score = result.output.Score });
            }



            #endregion


            #region OcitajCategory

            int predictedCategory;

            if (categories.Count() > 0)
                predictedCategory = categories.First().RecordPk;
            else
                predictedCategory = categoryIds.OrderBy(x => rand.Next()).First();


            var category = dBContext.tb_category.Where(x => x.category_pk.Equals(predictedCategory)).Select(x => new CategoryModel
            {
                RecordPk = x.category_pk,
                CategoryName = x.category_name,
                DepartmentFk = x.category_department_fk


            }).Single();


            category.Score = categories.Count() > 0 ? categories.Single(x => x.RecordPk.Equals(category.RecordPk)).Score : float.NaN;
            category.Score = Singoid(category.Score);

            var bytes = dBContext.tb_dept_cat_images.SingleOrDefault(x => x.dept_cat_category_fk.Value.Equals(category.RecordPk)).dept_cat_image_data;
            if (bytes != null)
            {
                category.ImageData = bytes;
                category.slika = "data:image/jpg;base64," + System.Convert.ToBase64String(category.ImageData);
            }
            else
                category.slika = defaultPhoto;


            homeRow2.TopCategory = category;
            #endregion OcitajCategory

            #region OcitajDepartmante


            int predictedDepartmant;

            if (departments.Count() > 0)
                predictedDepartmant = departments.First().RecordPk;
            else
                predictedDepartmant = departmentIds.OrderBy(x => rand.Next()).First();

            // predictedDepartmants = predictions.departments.Count() > 0 ? predictions.departments.OrderByDescending(x => x.Score).Take(4).Select(x => x.RecordPk).ToList()
            //  : departmentIds.OrderBy(x => rand.Next()).Take(4);

            var department = dBContext.tb_department.Where(x => x.department_pk.Equals(predictedDepartmant)).Select(x => new DepartmentModel
            {
                RecordPk = x.department_pk,
                DepartmentName = x.department_name,


            }).Single();


            department.Score = departments.Count > 0 ? departments.Single(x => x.RecordPk.Equals(department.RecordPk)).Score : float.NaN;
            department.Score = Singoid(department.Score);

            bytes = dBContext.tb_dept_cat_images.SingleOrDefault(x => x.dept_cat_department_fk.Value.Equals(department.RecordPk)).dept_cat_image_data;
            if (bytes != null)
            {
                department.ImageData = bytes;
                department.slika = "data:image/jpg;base64," + System.Convert.ToBase64String(department.ImageData);
            }
            else
                department.slika = defaultPhoto;

            homeRow2.TopDepartment = department;


            #endregion OcitajDepartmante

            #region Ocitaj Proizvode


            int predictedProduct;

            if (products.Count() > 0)
                predictedProduct = products.OrderByDescending(x => x.Score).First().ProductPk;
            else
                predictedProduct = productsIds.OrderBy(x => rand.Next()).First();





            var product = dBContext.tb_product.Where(x => x.product_pk.Equals(predictedProduct)).Select(x => new ProductModel
            {
                ProductName = x.product_name,
                ProductPrice = x.product_price,
                ProductPk = x.product_pk
            }).Single();


            product.Score = products.Count() > 0 && products.Any(x => x.ProductPk.Equals(product.ProductPk))? products.Single(x => x.ProductPk.Equals(product.ProductPk)).Score : float.NaN;
            product.Score = Singoid(product.Score);

            var slika = dBContext.tb_product_images.FirstOrDefault(x => x.prod_id == product.ProductPk && x.prod_main_photo);
            if (slika != null)
                product.slika = "data:image/jpg;base64," + System.Convert.ToBase64String(slika.prod_image_data);
            else
                product.slika = defaultPhoto;



            var topsale = TopSale();

            topsale.Score = products.Count() > 0 && products.Any(x => x.ProductPk.Equals(topsale.ProductPk)) ? products.Single(x => x.ProductPk.Equals(topsale.ProductPk)).Score : float.NaN;
            topsale.Score = Singoid(topsale.Score);

            var top_sale_img = dBContext.tb_product_images.FirstOrDefault(x => x.prod_id == topsale.ProductPk && x.prod_main_photo);
            if (top_sale_img != null)
                topsale.slika = "data:image/jpg;base64," + System.Convert.ToBase64String(top_sale_img.prod_image_data);
            else
                topsale.slika = defaultPhoto;

            homeRow2.TopProduct = product;
            homeRow2.TopSale = topsale;


            #endregion Ocitaj Proizvode






            return homeRow2;
        }
        public ProductModel TopSale()
        {
           var productIds = dBContext.tb_product.Select(x => x.product_pk).ToList();

            var topsales = new List<TopSaleModel>();
            foreach (var id in productIds)
            {
                var topsale = dBContext.tb_product_sale.Where(x => x.product_sale_product_fk == id).Sum(x => (int?) x.product_sale_count)??0;

                topsales.Add(new TopSaleModel { Count = topsale, ProductFk = id });
    
            }
            var item = topsales.OrderByDescending(x => x.Count).First();

            var product = dBContext.tb_product.Where(x => x.product_pk.Equals(item.ProductFk)).Select(x => new ProductModel
            {
                ProductName = x.product_name,
                ProductPrice = x.product_price,
                ProductPk = x.product_pk
            }).Single();

            return product;

        }

        //Iz top department omiljeni proizvodi
        public List<ProductModel> GetRow3(int user_id,int department_id)
        {
            var rand = new Random();
            var categories = dBContext.tb_category.Where(x => x.category_department_fk.Equals(department_id)).Select(x => x.category_pk).ToList() ;
            var productsIds = dBContext.tb_product.Where(x=> categories.Contains(x.product_category_fk)).Select(x => x.product_pk).ToList();


            var predictedProducts = new List<int>();        
            predictedProducts = productsIds.OrderBy(x => rand.Next()).Take(4).ToList();

            var products = dBContext.tb_product.Where(x => predictedProducts.Contains(x.product_pk)).Select(x => new ProductModel
            {
                ProductName = x.product_name,
                ProductPrice = x.product_price,
                ProductPk = x.product_pk
            }).ToList();

            foreach (var item in products)
            {
                item.Score = float.NaN;
                var slika = dBContext.tb_product_images.FirstOrDefault(x => x.prod_id == item.ProductPk && x.prod_main_photo);
                if (slika != null)
                    item.slika = "data:image/jpg;base64," + System.Convert.ToBase64String(slika.prod_image_data);
                else
                    item.slika = defaultPhoto;
            }
 

            return products;
        }
        //row4
        public HomeRow4Model GetProductsForTopCategory(int user_id,int category_id)
        {
            var HomeRow4Model = new HomeRow4Model();

            HomeRow4Model.CategoryName = dBContext.tb_category.Single(x => x.category_pk == category_id).category_name;

            var rand = new Random();

            #region Predict Products          

            var consumeModel = new ConsumeProductModel();

            var predicted_products = new List<ProductModel>();


            var productsIds = dBContext.tb_product.Where(x=> x.product_category_fk.Equals(category_id))
                .Select(x => x.product_pk).ToList();
            foreach (var id in productsIds)
            {
                var result = consumeModel.Predict(
                   new ModelInput()
                   {
                       UserId = user_id,
                       ProductId = id,
                   });
                if (result.valid && !float.IsNaN(result.output.Score))
                    predicted_products.Add(new ProductModel { ProductPk = id, Score = result.output.Score });
            }




            #endregion


            #region Ocitaj Proizvode


            var predictedProducts = new List<int>();

            if (predicted_products.Count() > 0)
            {
                if (predicted_products.Count() < 10)
                {
                    int count = predicted_products.Count();

                    predictedProducts = predicted_products.OrderByDescending(x => x.Score).Select(x => x.ProductPk).ToList();
                    predictedProducts.AddRange(productsIds.Except(predictedProducts).OrderBy(x => rand.Next()).Take(10 - count));
                }
                else
                    predictedProducts = predicted_products.OrderByDescending(x => x.Score).Take(10).Select(x => x.ProductPk).ToList();
            }
            else
                predictedProducts = productsIds.OrderBy(x => rand.Next()).Take(10).ToList();






            var products = dBContext.tb_product.Where(x => predictedProducts.Contains(x.product_pk)).Select(x => new ProductModel
            {
                ProductName = x.product_name,
                ProductPrice = x.product_price,
                ProductPk = x.product_pk
            }).ToList();

            foreach (var item in products)
            {


                item.Score = predicted_products.Any(x => x.ProductPk.Equals(item.ProductPk)) ? predicted_products.Single(x => x.ProductPk.Equals(item.ProductPk)).Score : float.NaN;
                item.Score = Singoid(item.Score);

                var slika = dBContext.tb_product_images.FirstOrDefault(x => x.prod_id == item.ProductPk && x.prod_main_photo);
                if (slika != null)
                    item.slika = "data:image/jpg;base64," + System.Convert.ToBase64String(slika.prod_image_data);
                else
                    item.slika = defaultPhoto;


            }
            HomeRow4Model.products = products.OrderByDescending(x => x.Score).ToList();


            #endregion Ocitaj Proizvode

            return HomeRow4Model;
        }
        //Best products under filter price
        public HomeRow5Model GetRow5(int user_id)
        {
            var row5 = new HomeRow5Model();
            row5.ProductPrice = 50;

            var rand = new Random();


            #region Predict Products          

            var consumeModel = new ConsumeProductModel();

            var predictions = new List<ProductModel>();


            var productsIds = dBContext.tb_product.Where(x=>x.product_price < row5.ProductPrice).Select(x => x.product_pk).ToList();
            foreach (var id in productsIds)
            {
                var result = consumeModel.Predict(
                   new ModelInput()
                   {
                       UserId = user_id,
                       ProductId = id,
                   });
                if (result.valid && !float.IsNaN(result.output.Score))
                    predictions.Add(new ProductModel { ProductPk = id, Score = result.output.Score });
            }


            #endregion



            #region Ocitaj Proizvode


            var predictedProducts = new List<int>();

            if (predictions.Count > 0)
            {
                if (predictions.Count() < 4)
                {
                    int count = predictions.Count();

                    predictedProducts = predictions.OrderByDescending(x => x.Score).Select(x => x.ProductPk).ToList();
                    predictedProducts.AddRange(productsIds.Except(predictedProducts).OrderBy(x => rand.Next()).Take(4 - count));
                }
                else
                    predictedProducts = predictions.OrderByDescending(x => x.Score).Take(4).Select(x => x.ProductPk).ToList();
            }
            else
                predictedProducts = productsIds.OrderBy(x => rand.Next()).Take(4).ToList();





            var products = dBContext.tb_product.Where(x => predictedProducts.Contains(x.product_pk)).Select(x => new ProductModel
            {
                ProductName = x.product_name,
                ProductPrice = x.product_price,
                ProductPk = x.product_pk
            }).ToList();

            foreach (var item in products)
            {


                item.Score = predictions.Any(x => x.ProductPk.Equals(item.ProductPk)) ? predictions.Single(x => x.ProductPk.Equals(item.ProductPk)).Score : float.NaN;
                item.Score = Singoid(item.Score);

                var slika = dBContext.tb_product_images.FirstOrDefault(x => x.prod_id == item.ProductPk && x.prod_main_photo);
                if (slika != null)
                    item.slika = "data:image/jpg;base64," + System.Convert.ToBase64String(slika.prod_image_data);
                else
                    item.slika = defaultPhoto;


            }
            row5.products = products.OrderByDescending(x => x.Score).ToList();


            #endregion Ocitaj Proizvode

            return row5;
        }



        public List<ProductModel> GetRow6(SearchModel model)
        {
            var rand = new Random();

            model.categoryList = GetPrivateCategories(model.userId);
            #region Predict Products          

            var consumeModel = new ConsumeProductModel();

            var predicted_products = new List<ProductModel>();

            foreach (var cat in model.categoryList)
            {
                var tempList = new List<ProductModel>();

                var Ids = dBContext.tb_product.Where(x=>x.product_category_fk == cat.RecordPk).Select(x => x.product_pk).ToList();
                foreach (var id in Ids)
                {
                    var result = consumeModel.Predict(
                       new ModelInput()
                       {
                           UserId = model.userId,
                           ProductId = id,
                       });
                    if (result.valid && !float.IsNaN(result.output.Score))
                        tempList.Add(new ProductModel { ProductPk = id, Score = result.output.Score });
                }
                predicted_products.AddRange(tempList.OrderByDescending(x => x.Score).Take(2).ToList());

            }

            var productsIds = dBContext.tb_product.Select(x => x.product_pk).ToList();

            #endregion


            #region Ocitaj Proizvode


            var predictedProducts = new List<int>();

            if (predicted_products.Count() > 0)
            {
                if (predicted_products.Count() < 10)
                {
                    int count = predicted_products.Count();

                    predictedProducts = predicted_products.OrderByDescending(x => x.Score).Select(x => x.ProductPk).ToList();
                    predictedProducts.AddRange(productsIds.Except(predictedProducts).OrderBy(x => rand.Next()).Take(10 - count));
                }
                else
                    predictedProducts = predicted_products.OrderByDescending(x => x.Score).Take(10).Select(x => x.ProductPk).ToList();
            }
            else
                predictedProducts = productsIds.OrderBy(x => rand.Next()).Take(10).ToList();






            var products = dBContext.tb_product.Where(x => predictedProducts.Contains(x.product_pk)).Select(x => new ProductModel
            {
                ProductName = x.product_name,
                ProductPrice = x.product_price,
                ProductPk = x.product_pk
            }).ToList();

            foreach (var item in products)
            {


                item.Score = predicted_products.Any(x => x.ProductPk.Equals(item.ProductPk)) ? predicted_products.Single(x => x.ProductPk.Equals(item.ProductPk)).Score : float.NaN;
                item.Score = Singoid(item.Score);

                var slika = dBContext.tb_product_images.FirstOrDefault(x => x.prod_id == item.ProductPk && x.prod_main_photo);
                if (slika != null)
                    item.slika = "data:image/jpg;base64," + System.Convert.ToBase64String(slika.prod_image_data);
                else
                    item.slika = defaultPhoto;


            }
            products = products.OrderByDescending(x => x.Score).ToList();

          


            #endregion Ocitaj Proizvode  
            
            return products;
        }
        public SharedModel SetDepartmentML(SearchModel model)
        {

            SharedModel sharedModel = new SharedModel();
            ModelBuilder modelBuilder = new ModelBuilder();


            var sendModel = new SendModel(dBContext.Database.Connection, con_string, typeof(tb_department_ml));


            sharedModel.errors = new List<string>();
            try
            {
                var department_ml = dBContext.tb_department_ml.SingleOrDefault(x => x.department_ml_department_fk == model.departmentFk && x.department_ml_user_fk == model.userId);
                if (department_ml != null)
                {
                    var value = (float) Math.Ceiling(department_ml.department_ml_count);
                    department_ml.department_ml_count = ++value;
                }
                else
                {
                    var newRecord = new tb_department_ml
                    {
                        department_ml_count = 1,
                        department_ml_user_fk = model.userId,
                        department_ml_department_fk = model.departmentFk
                    };
                    dBContext.tb_department_ml.Add(newRecord);

                }
                dBContext.SaveChanges();


                //stabilization
              
                var department_ml_list = dBContext.tb_department_ml.Where(x => x.department_ml_user_fk == model.userId)
                .Select(x => new DepartmentMLModel
                {
                    RecordPk = x.department_ml_pk,
                    DepartmentFk = x.department_ml_department_fk,
                    UserFk = x.department_ml_user_fk,
                    ItemCount = x.department_ml_count
                }).ToList();

                var duplicates = department_ml_list.GroupBy(s => Math.Ceiling(s.ItemCount))
                            .Where(g => g.Count() > 1)
                            .SelectMany(g => g).ToList();

                var fragment = new List<DepartmentMLModel>();

                if (department_ml != null)
                     fragment = duplicates.Where(x => Math.Ceiling(x.ItemCount).Equals(department_ml.department_ml_count) && x.RecordPk != department_ml.department_ml_pk).ToList();
                else
                    fragment = duplicates.Where(x => Math.Ceiling(x.ItemCount).Equals(1) && x.DepartmentFk != model.departmentFk).ToList();
  
                for (int i = 0; i < fragment.Count(); i++)
                {
                    float value;
                    if (department_ml != null)
                         value = (float) Math.Ceiling(department_ml.department_ml_count) - (float)((i+1) * 0.1);
                    else
                         value = duplicates.SingleOrDefault(x => x.DepartmentFk == model.departmentFk).ItemCount - (float)((i+1) * 0.1);
                                    
                     var pk = fragment[i].RecordPk;
                     var item = dBContext.tb_department_ml.SingleOrDefault(x => x.department_ml_pk == pk);
                     item.department_ml_count = value;
                }
                dBContext.SaveChanges();




                sharedModel.Status = Models.myEnums.CreateStatus.sucess;              
                
                modelBuilder.UpdateDepartment(sendModel);               




            }
            catch (Exception ex)
            {
                sharedModel.errors.Add(ex.Message);
                sharedModel.Status = Models.myEnums.CreateStatus.failed;

            }

            return sharedModel;

        }
        public SharedModel SetCategoryML(SearchModel model)
        {
            SharedModel sharedModel = new SharedModel();
            ModelBuilder modelBuilder = new ModelBuilder();

            var sendModel = new SendModel(dBContext.Database.Connection, con_string, typeof(tb_category_ml));

            sharedModel.errors = new List<string>();
            try
            {
                var category_ml = dBContext.tb_category_ml.SingleOrDefault(x => x.category_ml_category_fk == model.categoryFk && x.category_ml_user_fk == model.userId);
                if (category_ml != null)
                {
                    var value = (float)Math.Ceiling(category_ml.category_ml_count);
                    category_ml.category_ml_count = ++value;
                }
                else
                {
                    var newRecord = new tb_category_ml
                    {
                        category_ml_count = 1,
                        category_ml_user_fk = model.userId,
                        category_ml_category_fk = model.categoryFk
                    };
                    dBContext.tb_category_ml.Add(newRecord);
                }
                dBContext.SaveChanges();

                //stabilization

                var category_ml_list = dBContext.tb_category_ml.Where(x => x.category_ml_user_fk == model.userId)
                .Select(x => new CategoryMLModel
                {
                    RecordPk = x.category_ml_pk,
                    CategoryFk = x.category_ml_category_fk,
                    UserFk = x.category_ml_user_fk,
                    ItemCount = x.category_ml_count
                }).ToList();

                var duplicates = category_ml_list.GroupBy(s => Math.Ceiling(s.ItemCount))
                            .Where(g => g.Count() > 1)
                            .SelectMany(g => g).ToList();

                var fragment = new List<CategoryMLModel>();

                if (category_ml != null)
                    fragment = duplicates.Where(x => Math.Ceiling(x.ItemCount).Equals(category_ml.category_ml_count) && x.RecordPk != category_ml.category_ml_pk).ToList();
                else
                    fragment = duplicates.Where(x => Math.Ceiling(x.ItemCount).Equals(1) && x.CategoryFk != model.categoryFk).ToList();

                int count = fragment.Count();
                for (int i = 0; i < count; i++)
                {
                    float value;
                    if (category_ml != null)
                        value = (float)Math.Ceiling(category_ml.category_ml_count) - (float)((i + 1) * 0.1);
                    else
                        value = duplicates.SingleOrDefault(x => x.CategoryFk == model.categoryFk).ItemCount - (float)((i + 1) * 0.1);



                    var pk = fragment[i].RecordPk;
                    var item = dBContext.tb_category_ml.SingleOrDefault(x => x.category_ml_pk == pk);
                    item.category_ml_count = value;
                    //historyItems.Single(x => x.HistoryItemPk == fragment[i].HistoryItemPk).ModifiedRating = fragment[i].ModifiedRating;
                }
                dBContext.SaveChanges();


                modelBuilder.UpdateCategory(sendModel);

                sharedModel.Status = Models.myEnums.CreateStatus.sucess;
            }
            catch (Exception ex)
            {
                sharedModel.errors.Add(ex.Message);
                sharedModel.Status = Models.myEnums.CreateStatus.failed;

            }

            return sharedModel;
        }
       

      
    
    }
}