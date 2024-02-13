using ISProjectSchool.DataBase;
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
using ISProjectSchool.Models;

namespace ISProjectSchool.AddEditGroupWindow
{
    /// <summary>
    /// Логика взаимодействия для EditGroupWindow.xaml
    /// </summary>
    public partial class EditGroupWindow : Window
    {
        DatabaseHelper DB = new DatabaseHelper();
        public ObservableCollection<Subjects> Subjects { get; set; }
        private int GroupID { get; set; }

        public EditGroupWindow(int groupId)
        {
            InitializeComponent();
            GroupID = groupId;
            LoadSubjects();
            LoadExistingGroupData();
        }

        private void LoadSubjects()
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

            listBoxSubjects.ItemsSource = Subjects;
        }

        private void LoadExistingGroupData()
        {
            // Load group name
            string groupNameQuery = "SELECT GroupName FROM [dbo].[Groups] WHERE GroupID = @GroupID";
            SqlCommand cmd = new SqlCommand(groupNameQuery, DB.getConnection());
            cmd.Parameters.AddWithValue("@GroupID", GroupID);
            DB.openConnection();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    txtGroupName.Text = reader["GroupName"].ToString();
                }
            }
            DB.closeConnection();

            // Load selected subjects
            string selectedSubjectsQuery = "SELECT SubjectID FROM [dbo].[GroupSubjects] WHERE GroupID = @GroupID";
            cmd = new SqlCommand(selectedSubjectsQuery, DB.getConnection());
            cmd.Parameters.AddWithValue("@GroupID", GroupID);
            DB.openConnection();

            var selectedSubjectIds = new List<int>();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    selectedSubjectIds.Add((int)reader["SubjectID"]);
                }
            }
            DB.closeConnection();

            foreach (var subject in listBoxSubjects.Items)
            {
                var sub = subject as Subjects;
                if (selectedSubjectIds.Contains(sub.SubjectID))
                {
                    listBoxSubjects.SelectedItems.Add(subject);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtGroupName.Text))
            {
                MessageBox.Show("Заполните все поля", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Update group name
            string updateGroupNameQuery = "UPDATE [dbo].[Groups] SET GroupName = @GroupName WHERE GroupID = @GroupID";
            SqlCommand command = new SqlCommand(updateGroupNameQuery, DB.getConnection());
            command.Parameters.AddWithValue("@GroupName", txtGroupName.Text);
            command.Parameters.AddWithValue("@GroupID", GroupID);
            DB.openConnection();
            command.ExecuteNonQuery();
            DB.closeConnection();

            // Update subjects
            string deleteSubjectsQuery = "DELETE FROM [dbo].[GroupSubjects] WHERE GroupID = @GroupID";
            command = new SqlCommand(deleteSubjectsQuery, DB.getConnection());
            command.Parameters.AddWithValue("@GroupID", GroupID);
            DB.openConnection();
            command.ExecuteNonQuery();
            DB.closeConnection();

            string insertSubjectQuery = "INSERT INTO [dbo].[GroupSubjects] ([GroupID], [SubjectID]) VALUES (@GroupID, @SubjectID)";
            command = new SqlCommand(insertSubjectQuery, DB.getConnection());
            foreach (Subjects selectedSubject in listBoxSubjects.SelectedItems)
            {
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@GroupID", GroupID);
                command.Parameters.AddWithValue("@SubjectID", selectedSubject.SubjectID);
                DB.openConnection();
                command.ExecuteNonQuery();
                DB.closeConnection();
            }

            MessageBox.Show("Изменения сохранены", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}