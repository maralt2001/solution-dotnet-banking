using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ServiceApiData
{
    public class DataProtection
    {

        [Required(ErrorMessage = "key is required")]
        public string key { get; set; }
        [Required(ErrorMessage = "data is required")]
        public object data;

       
    }
}
