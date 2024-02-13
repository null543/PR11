using ISProjectSchool.DataBase;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ISProjectSchool
{
    /// <summary>
    /// Логика взаимодействия для Schedules.xaml
    /// </summary>
    public partial class Schedules : Window
    {
        DatabaseHelper DB = new DatabaseHelper();
        public Schedules()
        {
            InitializeComponent();
            LoadComboBoxes();
            FillComboBox(TeacherComboBox);
        }

        private void LoadComboBoxes()
        {
            DB.openConnection();
            // Создание объектов ComboBoxFiller для каждого ComboBox
            ComboBoxFiller subjectFiller = new ComboBoxFiller(DB.getConnection(), "SELECT SubjectID, SubjectName FROM Subjects");
            ComboBoxFiller groupFiller = new ComboBoxFiller(DB.getConnection(), "SELECT GroupID, GroupName FROM Groups");

            ComboBoxFiller roomFiller = new ComboBoxFiller(DB.getConnection(), "SELECT RoomID, RoomName FROM Rooms");

            // Заполнение ComboBoxes
            subjectFiller.FillComboBox(SubjectComboBox, "SubjectName", "SubjectID");
            groupFiller.FillComboBox(GroupComboBox, "GroupName", "GroupID");
            roomFiller.FillComboBox(RoomComboBox, "RoomName", "RoomID");
            DB.closeConnection();
        }

        public void FillComboBox(ComboBox comboBox)
        {
            DB.openConnection();
            using (SqlCommand command = new SqlCommand("SELECT UserID, FirstName, LastName, MiddleName FROM [dbo].[Users] WHERE RoleID = 2", DB.getConnection()))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string firstName = reader.GetString(reader.GetOrdinal("FirstName"));
                        string lastName = reader.GetString(reader.GetOrdinal("LastName"));
                        string middleName = reader.GetString(reader.GetOrdinal("MiddleName"));
                        int id = reader.GetInt32(reader.GetOrdinal("UserID"));

                        string fullName = $"{firstName} {lastName} {middleName}".Trim();
                        ComboBoxItem comboBoxItem = new ComboBoxItem()
                        {
                            Content = fullName,
                            Tag = id
                        };
                        comboBox.Items.Add(comboBoxItem);
                    }
                }
            }
            DB.closeConnection();
        }


        private void CreateScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Считывание данных из ComboBox
                int subjectId = (int)((ComboBoxItem)SubjectComboBox.SelectedItem).Tag;
                int groupId = (int)((ComboBoxItem)GroupComboBox.SelectedItem).Tag;
                int teacherId = (int)((ComboBoxItem)TeacherComboBox.SelectedItem).Tag;
                int roomId = (int)((ComboBoxItem)RoomComboBox.SelectedItem).Tag;

                // Обработка и преобразование даты и времени
                DateTime selectedDate = DatePick.SelectedDate ?? DateTime.Now;

                if (!DateTime.TryParse(StartTimeTextBox.Text, out DateTime startTime))
                {
                    MessageBox.Show("Неверный формат времени начала");
                    return;
                }

                // Установка времени окончания на 1 час 30 минут после начала
                DateTime endTime = startTime.Add(new TimeSpan(1, 30, 0));

                DateTime startDateTime = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, startTime.Hour, startTime.Minute, startTime.Second);
                DateTime endDateTime = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, endTime.Hour, endTime.Minute, endTime.Second);

                string dayOfWeek = selectedDate.DayOfWeek.ToString();

                // Создание SQL-запроса
                string query = @"
INSERT INTO Schedules (SubjectID, GroupID, UserID, RoomID, StartTime, EndTime, DayOfWeek)
VALUES (@SubjectID, @GroupID, @TeacherID, @RoomID, @StartTime, @EndTime, @DayOfWeek)";

                string con = "Server=WIN-N16HPOIVC32;Database=SchoolDB;Integrated Security=True;";

                // Выполнение SQL-запроса
                using (SqlConnection connection = new SqlConnection(con))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SubjectID", subjectId);
                        command.Parameters.AddWithValue("@GroupID", groupId);
                        command.Parameters.AddWithValue("@TeacherID", teacherId);
                        command.Parameters.AddWithValue("@RoomID", roomId);
                        command.Parameters.AddWithValue("@StartTime", startDateTime);
                        command.Parameters.AddWithValue("@EndTime", endDateTime);
                        command.Parameters.AddWithValue("@DayOfWeek", dayOfWeek);

                        connection.Open();
                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Расписание успешно создано!");
                        }
                        else
                        {
                            MessageBox.Show("Ошибка при создании расписания.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
        }


    }
}
