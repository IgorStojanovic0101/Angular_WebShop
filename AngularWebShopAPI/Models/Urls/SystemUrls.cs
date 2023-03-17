using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models.Urls
{
    public class SystemUrls
    { 
        public class User
        {
            private const string pref = "User/";  
            public const string GetUsers = pref + "GetUsers";
            public const string FindUsers = pref + "FindUsers";

        }
        public class Temp
        {
            private const string pref = "Temp/";
            public const string GetUsers = pref + "GetUsers";
            public const string FindUsers = pref + "FindUsers";

        }
    }
}