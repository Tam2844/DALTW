﻿using System.ComponentModel.DataAnnotations;

namespace DALTW.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        [Required, StringLength(50)]
        public string Name { get; set; }
        public ICollection<Document>? Documents { get; set; } 
    }
}
