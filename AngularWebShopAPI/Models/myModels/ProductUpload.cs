using Api.Models.myEnums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Api.Models.myModels
{
    public class ProductUpload
    {
    
        public int ProductPk;
        public string ProductName;
        public string ProductDetails;
        public int ProductPrice;
        public int CategoryFk;
        public string Base64File;
        public bool MainPhoto;
        public ProductStatus productStatus;
    }
}