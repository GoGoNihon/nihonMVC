using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoGoNihon_MVC.Models
{

    public enum scheudleTypes
    {
        morning,
        afternoon,
        night

    }


    public class CourseSchedule
    {
        [Key]
        [Required]
        public int id { get; set; }

        public int type { get; set; }
        public DateTime from { get; set; }
        public DateTime to { get; set; }

        public int schoolCourseID { get; set; }

        [ForeignKey("schoolCourseID")]
        public virtual SchoolCourse schoolCourse { get; set; }
    }
}