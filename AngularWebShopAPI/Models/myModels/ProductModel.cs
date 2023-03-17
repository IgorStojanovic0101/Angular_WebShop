using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace Api.Models.myModels
{
    public class ProductModel
    {
        public int ProductPk { get; set; }
        public int ProductFk { get; set; }
        public int CategoryFk { get; set; }
        public string ProductName { get; set; }
        public string ProductDetails { get; set; }
        public string ProductImageName { get; set; }
        public int ProductPrice { get; set; }
        public byte[] ImageData { get; set; }
        public List<ImageModel> Images { get; set; }
        public ImageModel current_image { get; set; }
        public string slika { get; set; }

        public int temp { get; set; }

        public int Rating;
        public string[] RatingStar;

        public float Score;
        public int cols;
        public int rows;

    }
}