using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISProjectSchool.Models
{
    public class Groups
    {
        public int GroupID { get; set; }
        public string GroupName { get; set; }

        // This will store individual subjects
        public List<string> Subjects { get; set; }

        // This will store the subjects in a comma-separated string
        public string SubjectsString { get; set; }

        public Groups()
        {
            // Initialize the Subjects list in the constructor
            Subjects = new List<string>();
        }
    }
}

