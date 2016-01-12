using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GoGoNihon_MVC.Models
{
    public class CourseLength
    {
        [Key]
        public int id { get; set; }

        public int courseID { get; set; }

        [ForeignKey("courseID")]
        public virtual SchoolCourse schoolCourse { get; set; }

        public bool withVisa { get; set; }
        public int from { get; set; }
        public int to { get; set; }

    }
}