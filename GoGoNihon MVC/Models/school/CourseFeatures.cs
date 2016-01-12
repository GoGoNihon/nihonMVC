using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoGoNihon_MVC.Models
{
    public class CourseFeatures
    {
        //[Key]
        //[Required]
        //public int id { get; set; }

        public bool jlptClasses { get; set; }
        public bool conversationClasses { get; set; }
        public bool openClasses { get; set; }
        public bool higherEducationPrep { get; set; }
        public bool businessClasses { get; set; }
        public bool cultureClasses { get; set; }
        public bool kansaiStudies { get; set; }
        public bool currentEventsClasses { get; set; }
        public bool movieClasses { get; set; }

        //public int schoolCourseID { get; set; }

        //[ForeignKey("schoolCourseID")]
        //public virtual SchoolCourse schoolCourse { get; set; }
    }
}