using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Linq;
using Api.Models.myModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Web.Http.Cors;
using Api.BusinessLogic;

namespace Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class ProductsController : BaseController<Products>
    {
        public Products product = new Products();



        [HttpGet]
        public List<ProductModel> GetProducts() => product.GetProducts();

        [HttpGet]
        public List<ProductModel> GetGroupProducts() => product.GetGroupProducts();


        [HttpPost]
        public List<ProductModel> GetProductbycode(int id) => product.GetProductbycode(id);

        [HttpPost]
        public ProductModel GetProductbyId(int id) => product.GetProductbyId(id);

        [HttpPost]
        public string GetPrtbyId(int id)
        {

            Call<string>(x => x.GetGroupProducts());

            return null;
        }

        [HttpPost]
        public ProductsPageModel SearchProducts(SearchModel model) => product.SearchProducts(model);
        
        [HttpPost]
        public ProductsPageModel GetProductsByCategoryFk(SearchModel model) => product.GetProductsByCategoryFk(model);

        [HttpPost]
        public List<ProductModel> GetProductListByCategoryFk(int category_id) => product.GetProductListByCategoryFk(category_id);

        [HttpPost]
        public ProductsPageModel GetProductsByDepartmentFk(SearchModel model) => product.GetProductsByDepartmentFk(model);
       
        
        [HttpPost]
        public ProductsPageModel FilterProducts(FilterModel filter) =>  product.FilterProducts(filter);

        [HttpPost]
        public SharedModel SetProduct(ProductUpload productUpload) => product.SetProduct(productUpload);

    }
}   