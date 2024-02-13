using ISProjectSchool.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using ISProjectSchool.Models;

namespace ISProjectSchool
{
    /// <summary>
    /// Логика взаимодействия для TeacherWindow.xaml
    /// </summary>
    public partial class TeacherWindow : Window
    {
        DatabaseHelper DB = new DatabaseHelper();
        public string Email;
        public TeacherWindow(string email)
        {
            InitializeComponent();
            this.Email = email;
            LoadLessonsForToday();


        }


        private void LoadLessonsForToday()
        {
            string userid = DB.GetUserIDToEmail(Email);
            DB.openConnection();
            ComboBoxFiller subjectFiller = new ComboBoxFiller(DB.getConnection(), $@"SELECT ScheduleID, subj.SubjectName + ' (' + CONVERT(VARCHAR, s.StartTime, 108) + ')' AS LessonInfo
FROM Schedules s
JOIN Subjects subj ON s.SubjectID = subj.SubjectID
WHERE CAST(s.StartTime AS DATE) = CAST(GETDATE() AS DATE) AND s.UserID = {userid}");
            subjectFiller.FillComboBox(LessonsComboBox, "LessonInfo", "ScheduleID");
            DB.closeConnection();


        }

        private void LessonsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (LessonsComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string scheduleId = selectedItem.Tag.ToString();

                var lessonDetails = DatabaseHelper.GetLessonDetails(scheduleId);

                LessonDetailsListView.ItemsSource = lessonDetails;

            }
        }
        private void LessonDetailsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LessonDetailsListView.SelectedItem is ScheduleItem selectedLessonDetail)
            {
                string scheduleId = selectedLessonDetail.ScheduleID;
                LessonPanel lessonPanel = new LessonPanel(scheduleId);
                lessonPanel.Show();

            }
        }

    }
}
