using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.NET
{
    public class SendModel
    {
        public DbConnection dbConnection;
        public string conString;
        public Type tip { get; set; }
        public string user_name { get; set; }
        public string item_name { get; set; }

        public string label_name { get; set; }

        public SendModel(DbConnection dbConnection, string conString, Type tip)
        {
            this.dbConnection = dbConnection;
            this.conString = conString;
            this.tip = tip;
        }

       

    }
}
