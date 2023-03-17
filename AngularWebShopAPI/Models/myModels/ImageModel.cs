using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models.myModels
{
    public class ImageModel
    {
        public int RecordPk;
        public int RecordFk;
        public string type;
        public bool MainPhoto;
        public byte[] ImageData;
        public string src;
       }
}