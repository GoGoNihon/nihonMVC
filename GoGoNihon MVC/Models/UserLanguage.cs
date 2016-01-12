using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GoGoNihon_MVC.Models
{
    public class UserLanguage
    {
        [Required]
        [Key]
        public int userLanguageID { get; set; }

        [Required]
        public string code { get; set; }

        [ForeignKey("code")]
        public Language language { get; set; }

        [Required]
        public string userID { get; set; }

        [ForeignKey("userID")]
        public ApplicationUser user { get; set; }

    }
}