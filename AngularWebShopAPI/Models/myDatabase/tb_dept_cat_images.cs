//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Api.Models.myDatabase
{
    using System;
    using System.Collections.Generic;
    
    public partial class tb_dept_cat_images
    {
        public int dept_cat_pk { get; set; }
        public Nullable<int> dept_cat_department_fk { get; set; }
        public Nullable<int> dept_cat_category_fk { get; set; }
        public byte[] dept_cat_image_data { get; set; }
        public System.DateTime start_date { get; set; }
        public Nullable<System.DateTime> end_date { get; set; }
        public System.DateTime createdondate { get; set; }
        public string createdby { get; set; }
        public Nullable<System.DateTime> lastupdateddate { get; set; }
        public string lastupdatedby { get; set; }
    
        public virtual tb_category tb_category { get; set; }
        public virtual tb_department tb_department { get; set; }
    }
}
