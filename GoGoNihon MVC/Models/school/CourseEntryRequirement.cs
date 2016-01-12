using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoGoNihon_MVC.Models.school
{
    public class CourseEntryRequirement
    {
        [Key]
        [Required]
        public int id { get; set; }
        [Required]
        public string requirement { get; set; }

        public int schoolCourseID { get; set; }

        [ForeignKey("schoolCourseID")]
        public virtual SchoolCourse schoolCourse { get; set; }
    }
}