using ISProjectSchool.DataBase;
using ISProjectSchool.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ISProjectSchool.AddEditGroupWindow
{
    /// <summary>
    /// Логика взаимодействия для AddGroupWindow.xaml
    /// </summary>
    public partial class AddGroupWindow : Window
    {
        DatabaseHelper DB = new DatabaseHelper();
        public AddGroupWindow()
        {
            InitializeComponent();
            LoadSubjects();
        }

        public ObservableCollection<Subjects> Subjects { get; set; }
        public void LoadSubjects()
        {
            Subjects = new ObservableCollection<Subjects>();

            string query = "SELECT SubjectID, SubjectName FROM [dbo].[Subjects]";


            SqlCommand cmd = new SqlCommand(query, DB.getConnection());

            DB.openConnection();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Subjects.Add(new Subjects
                    {
                        SubjectID = (int)reader["SubjectID"],
                        SubjectName = reader["SubjectName"].ToString()
                    });
                }
            }
            DB.closeConnection();


            listBoxSubjects.ItemsSource = Subjects; // Привязка коллекции к ListBox
        }

        //получение ID выбранных элементов из листа с предметами
        public List<int> GetSelectedSubjectsIDs()
        {
            List<int> selectedSubjectIDs = new List<int>();

            foreach (Subjects selectedSubject in listBoxSubjects.SelectedItems)
            {
                selectedSubjectIDs.Add(selectedSubject.SubjectID);
            }

            return selectedSubjectIDs;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtGroupName.Text))
            {
                MessageBox.Show("Заполните все поля","Уведомление",MessageBoxButton.OK,MessageBoxImage.Information);
            }
            else
            {
                List<int> idsubject = GetSelectedSubjectsIDs();
                string query = @"
INSERT INTO [dbo].[Groups] (GroupName) VALUES (@GroupName);
SELECT SCOPE_IDENTITY();";

                int newGroupId = -1; // Переменная для хранения нового ID

                using (SqlCommand command = new SqlCommand(query, DB.getConnection()))
                {
                    command.Parameters.AddWithValue("@GroupName", txtGroupName.Text);
                    DB.openConnection();
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        newGroupId = Convert.ToInt32(result);
                    }
                    DB.closeConnection();
                }

                string queryGroupSubjects = @"
INSERT INTO [dbo].[GroupSubjects] ([GroupID], [SubjectID]) values (@GroupID1, @SubjectID1) ;";
                using (SqlCommand command = new SqlCommand(queryGroupSubjects, DB.getConnection()))
                {

                    foreach (int subjectid in idsubject)
                    {
                        command.Parameters.Clear(); // Очищаем параметры от предыдущих значений

                        command.Parameters.AddWithValue("@GroupID1", newGroupId);
                        command.Parameters.AddWithValue("@SubjectID1", subjectid);

                        DB.openConnection();
                        command.ExecuteScalar();
                        DB.closeConnection();
                    }

                }

                MessageBox.Show($"Группа была успешно добавлена!","Уведомление",MessageBoxButton.OK,MessageBoxImage.Information) ;


            }
        }
    }


}
