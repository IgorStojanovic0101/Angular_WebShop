using Api.Models.myEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models.myModels
{
    public class FilterModel
    {
        public bool Ten_to_20;
        public bool Twenty_to_30;

        public bool radio;
        public int stars;
        public int categoryFk;
        public int departmentFk;
        public bool search;
        public string search_input;
        public FilterStatus status;

    }
}