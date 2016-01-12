using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GoGoNihon_MVC.Models
{
    public class ContentBody
    {
        [Key]
        public int contentBodyID { get; set; }
        [Required]
        public int contentID { get; set; }
        [Required]
        public string code { get; set; }

        [ForeignKey("code")]
        public Language language { get; set; }

        public string body { get; set; }
        public DateTime lastModified { get; set; }

        
        public string lastModifiedByID { get; set; }

    }
}