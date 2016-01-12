using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GoGoNihon_MVC.Models
{
    public class Language
    {

        [Key]
        [Required]
        public string code { get; set; }


        public string name { get; set; }
        public string flag { get; set; }

    }
}