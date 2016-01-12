using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoGoNihon_MVC.Models
{

    public enum courseTypes
    {
        general,
        conversation

    }

    public class SchoolStation
    {
        [Key]
        [Required]
        public int id { get; set; }
        [Required]
        public string name { get; set; }
       
        public string image { get; set; }
        [Required]
        public string line { get; set; }
        [Required]
        public string distance { get; set; }
        [Required]
        public int schoolID { get; set; }

    }
}