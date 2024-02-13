using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISProjectSchool.Models
{
    public class ScheduleItem
    {
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string SubjectName { get; set; }
        public string TeacherName { get; set; }
        public string RoomName { get; set; }
        public string GroupName { get; set; }
        public string DayOfWeek { get; set; }

        public string ScheduleID { get; set; }

        public string StartTimeFormatted => StartTime.ToString("HH:mm");
        public string EndTimeFormatted => EndTime.ToString("HH:mm");
        public string DateFormatted => StartTime.ToString("yyyy-MM-dd");
    }


}
