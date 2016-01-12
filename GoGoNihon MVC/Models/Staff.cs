using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoGoNihon_MVC.Models
{
    public class Staff
    {
        public class StaffMember
        {
            public string Imgurl { get; set; }
            public string Flag { get; set; }
            public string Name { get; set; }

            public string TitleEN { get; set; }
            public string TitleIT { get; set; }
            public string TitleSE { get; set; }
            public string TitleES { get; set; }
            public string TitleDE { get; set; }

            public StaffMember()
            {
                Imgurl = Flag = Name = TitleDE = TitleEN = TitleES = TitleIT = TitleSE = "null";
            }

            public StaffMember(string name, string flag, string img, string en, string it, string se, string es, string de)
            {
                Imgurl = img;
                Flag = flag;
                Name = name;

                TitleEN = en;
                TitleIT = it;
                TitleSE = se;
                TitleES = es;
                TitleDE = de;
            }

        }

        public List<StaffMember> lstStaff { get; set; }

        public StaffMember SnowflakeLeft { get; set; }
        public StaffMember SnowflakeRight { get; set; }

        public string languageCode { get; set; }

        public Staff(string lang)
        {
            languageCode = lang;
            BuildStaff();
            SortStaff();
        }

        public Staff()
        {
            languageCode = "en";
            BuildStaff();
            SortStaff();
        }

        private void BuildStaff()
        {
            SnowflakeLeft = new StaffMember("Davide", "italy.png", "davide.jpg", "CEO", "CEO", "CEO", "CEO", "CEO");
            SnowflakeRight = new StaffMember("John", "sweden.png", "john.jpg", "Vice President", "Vice Presidente", "Vice President", "Vice Presidente", "Vice President");

            lstStaff = new List<StaffMember>();

            //All the staff
            lstStaff.Add(new StaffMember("Adrian", "spain.png", "adrian.jpg", "Spanish Student Coordinator", "Responsabile studenti", "Spansk student-koordinator", "Coordinadora de Estudiantes", "Student Coordinator für Spanien"));
            lstStaff.Add(new StaffMember("Alanna", "united-kingdom.png", "alanna.jpg", "International Student Coordinator", "Responsabile studenti", "Internationell student-koordinator", "Coordinadora de Estudiantes", "International Student Coordinator"));
            lstStaff.Add(new StaffMember("Andra", "italy.png", "andra.jpg", "Italian Student Coordinator", "Responsabile studenti", "Italienska student-koordinator", "Coordinadora de Estudiantes", "Student Coordinator"));
            lstStaff.Add(new StaffMember("Audrey", "usa.png", "audrey.jpg", "Media & Marketing Developer", "PR & Marketing", "Marknadsföringsansvarig", "Media & Marketing", "Media & Marketing Developer"));
            lstStaff.Add(new StaffMember("Azusa", "japan.png", "azusa.jpg", "School Coordinator Manager", "Manager", "Skol-koordinator manager", "Manager de Coordinadores de Escuelas", "School Coordinator Manager"));
            lstStaff.Add(new StaffMember("Vanessa", "germany.png", "vanessa.jpg", "German Student Coordinator", "Responsabile studenti", "Tysk student-koordinator", "Coordinadora de Estudiantes", "Student Coordinator für Deutschland"));
            //lstStaff.Add(new StaffMember("Camila", "sweden.png", "camila.jpg", "", "", "", "", ""));
            lstStaff.Add(new StaffMember("Catherine", "canada.png", "catherine.jpg", "International Student Coordinator", "Responsabile studenti", "Internationell student-koordinator", "Coordinadora de Estudiantes", "International Student Coordinator"));
            lstStaff.Add(new StaffMember("Chris", "usa.png", "chris.jpg", "Head of Operations", "COO", "Verksamhetsansvarig", "Respondable General", "Head of Operations"));
            lstStaff.Add(new StaffMember("Davina", "germany.png", "davina.jpg", "German Marketing Coordinator", "German Marketing Coordinator", "German Marketing Coordinator", "German Marketing Coordinator", "German Marketing Coordinator"));
            lstStaff.Add(new StaffMember("Eriko", "japan.png", "eriko.jpg", "Accounts", "Contabile", "Bokföring", "Contable", "Buchhaltung"));
            lstStaff.Add(new StaffMember("Fede", "italy.png", "fede.jpg", "Course Developer", "Responsabile corsi", "Kursutvecklare", "Desarrollador de Cursos", "Kursentwicklung"));
            lstStaff.Add(new StaffMember("Haruka", "japan.png", "haruka.jpg", "School Coordinator", "Coordinatrice scuole", "Skol-koordinator", "Coordinadora de Escuelas", "School Coordinator"));
            lstStaff.Add(new StaffMember("Jeanette", "sweden.png", "jeanette.jpg", "Swedish Student Coordinator", "Responsabile studenti", "Svensk student-koordinator", "Coordinadora de Estudiantes", "Student Coordinator für Schweden"));
            lstStaff.Add(new StaffMember("Kayoko", "japan.png", "kayoko.jpg", "Housing Coordinator", "Responsabile alloggi", "Boende-koordinator", "Coordinadora de Alojamientos", "Housing coordinator"));
            lstStaff.Add(new StaffMember("Kohei", "japan.png", "kohei.jpg", "Designer", "Designer", "Designer", "Diseñador", "Designer"));
            lstStaff.Add(new StaffMember("Mar", "spain.png", "mar.jpg", "Spanish Student Coordinator", "Responsabile studenti", "Spansk student-koordinator", "Coordinadora de Estudiantes", "Student Coordinator für Spanien"));
            lstStaff.Add(new StaffMember("Mike", "usa.png", "mike.jpg", "Web Developer", "Programmatore", "Webbutvecklare", "Desarrollador Web", "Web Developer"));
            lstStaff.Add(new StaffMember("Nathan", "australia.png", "none.png", "Web Developer", "Programmatore", "Webbutvecklare", "Desarrollador Web", "Web Developer"));
            lstStaff.Add(new StaffMember("Nayoung", "korea.png", "nayoung.jpg", "School Coordinator", "Responsabile alloggi", "Skol-koordinator", "Coordinadora de Escuelas", "School Coordinator"));
            lstStaff.Add(new StaffMember("Oskar", "sweden.png", "oskar.jpg", "Swedish Student Coordinator", "Responsabile studenti", "Svensk student-koordinator", "Coordinadora de Estudiantes", "Student Coordinator für Schweden"));
            lstStaff.Add(new StaffMember("Sabrina", "italy.png", "sabrina.jpg", "Student Coordinator Manager", "Manager", "Student-koordinator Manager", "Manager de Coordinadores de Estudiantes", "Student Coordinator Manager"));

        }

        private void SortStaff()
        {
            lstStaff = lstStaff.OrderBy(o => o.Name).ToList();
        }
    }
}