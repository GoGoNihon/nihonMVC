using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoGoNihon_MVC.Models
{
    public class SchoolFeatures
    {
        //[Key]
        //[Required]
        //public int id { get; set; }

        //public bool greatForWestern { get; set;}
        public bool acceptsBeginners { get; set; }
        public bool wifiInCommonAreas { get; set; }
        public bool studentLounge { get; set; }
        public bool studentLounge24hour { get; set; }
        public bool englishStaff { get; set; }
        public bool fullTimeJobSupport { get; set; }
        public bool partTimeJobSupport { get; set; }
        public bool interactWithJapanese { get; set; }
        public bool smallClassSizes { get; set; }
        public bool studentDorms { get; set; }
        public bool studentCafe { get; set; }

        //this even needed?....
        public bool uniqueFeature { get; set; }
        

    }
}