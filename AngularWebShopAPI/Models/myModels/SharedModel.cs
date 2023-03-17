using Api.Models.myEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
 
namespace Api.Models.myModels
{
    public class SharedModel
    {
       public List<string> errors;
       public int RecordPk;
       public CreateStatus Status;


    }
}