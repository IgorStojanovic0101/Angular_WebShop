using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Api.Models.myDatabase;

namespace Api.Models.myModels
{
    public class UserModel : tb_users
    {
        public int UserPk { get; set; }
        public string FirstName { get; set; }
         
        public string Username { get; set; }
        public string LastName { get; set; }
        public bool isAdmin { get; set; }

     
        public string Password
        {
            get;
            set;
        }
        public string CnfPassword
        {
            get;
            set;
        }
    }
}