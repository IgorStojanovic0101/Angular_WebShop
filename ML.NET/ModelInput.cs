using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;
namespace ML.NET
{
    public class ModelInput
    {
        [ColumnName("rating_user_fk"), LoadColumn(0)]
        public int UserId { get; set; }


        [ColumnName("rating_product_fk"), LoadColumn(1)]
        public int ProductId { get; set; }


        [ColumnName("rating_item"), LoadColumn(2)]
        public float Rating { get; set; }


    }

    public class DepartmentModelInput
    {
        [ColumnName("department_ml_user_fk")]
        public int UserId { get; set; }


        [ColumnName("department_ml_department_fk")]
        public int DepartmentId { get; set; }


        [ColumnName("department_ml_count")]
        public float Count { get; set; }


    }
    public class CategoryModelInput
    {
        [ColumnName("category_ml_user_fk"), LoadColumn(0)]
        public int UserId { get; set; }


        [ColumnName("category_ml_category_fk"), LoadColumn(1)]
        public int CategoryId { get; set; }


        [ColumnName("category_ml_count"), LoadColumn(2)]
        public float Count { get; set; }


    }
}
