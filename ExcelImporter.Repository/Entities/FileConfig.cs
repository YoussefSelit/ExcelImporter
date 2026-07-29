using System;
using System.Collections.Generic;
using System.Text;

namespace ExcelImporter.Repository.Entities
{
   
    public class FileConfig
    {
        public int Id { get; set; }
        public string Prefix { get; set; }
        public int? HeaderRow { get; set; }        
        public int? IgnoreFirstNoOfRows { get; set; }     
        public int? IgnoreFirstNoOfColumns { get; set; }
        public int? IgnoreLastNoOfRows { get; set; }
        public int? IgnoreLastNoOfColumns { get; set; }
    }
}


