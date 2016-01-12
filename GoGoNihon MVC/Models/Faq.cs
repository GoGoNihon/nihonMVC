using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace GoGoNihon_MVC.Models
{
    
    public class Faq
    {
        public int id { get; set; }
        public string languageCode { get; set; }
        public List<FaqQuestion> questions { get; set; }
        public string name { get; set; }


        public Faq()
        {
            languageCode = "en";
            questions = new List<FaqQuestion>();
        }

    }



}