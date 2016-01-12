using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GoGoNihon_MVC.Models.school
{
    public class CourseCost
    {
        [Key]
        [Required]
        public int id { get; set; }

        [Required]
        public string title { get; set; }

        //30days = 1 month
        [Required]
        public TimeSpan druation { get; set; }
        [Required]
        public decimal cost { get; set; }
        [Required]
        public int schoolCourseID { get; set; }

        [ForeignKey("schoolCourseID")]
        public virtual SchoolCourse schoolCourse { get; set; }
        
    }
}