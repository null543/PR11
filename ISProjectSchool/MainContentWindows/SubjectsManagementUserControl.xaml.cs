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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ISProjectSchool.AddEditGroupWindow;
using ISProjectSchool.AddEditSubjectWindow;
using ISProjectSchool.DataBase;
using ISProjectSchool.Models;

namespace ISProjectSchool.MainContentWindows
{
    /// <summary>
    /// Логика взаимодействия для SubjectsManagementUserControl.xaml
    /// </summary>
    public partial class SubjectsManagementUserControl : UserControl
    {
        DatabaseHelper DB = new DatabaseHelper();
        public SubjectsManagementUserControl()
        {
            InitializeComponent();
            LoadSubjectsData();
        }

        private void LoadSubjectsData()
        {
            List<Subjects> subjects = DatabaseHelper.LoadSubjectData();
            StudentsDataGrid.ItemsSource = subjects;
        }

        private void AddGroupButton_Click(object sender, RoutedEventArgs e)
        {
            AddSubjectWindow addSubjectWindow = new AddSubjectWindow();

            // Отобразить окно и ожидать его закрытия
            bool? dialogResult = addSubjectWindow.ShowDialog();

            // Проверить результат диалога (например, была ли нажата кнопка "ОК" или "Отмена")
            if (dialogResult == true)
            {
                LoadSubjectsData();
            }
            else
            {

                LoadSubjectsData();
            }
        }

        private void EditGroupButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedGroup = StudentsDataGrid.SelectedItem as ISProjectSchool.Models.Subjects;
            if (selectedGroup != null)
            {
                var idsubject = selectedGroup.SubjectID;
                var subjectname = selectedGroup.SubjectName;

                EditSubjectWindow editGroupWindow = new EditSubjectWindow(idsubject, subjectname);

                // Отобразить окно и ожидать его закрытия
                bool? dialogResult = editGroupWindow.ShowDialog();

                // Проверить результат диалога (например, была ли нажата кнопка "ОК" или "Отмена")
                if (dialogResult == true)
                {
                    LoadSubjectsData();
                }
                else
                {

                    LoadSubjectsData();
                }
            }
        }

        private void DeleteGroupButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedSubject = StudentsDataGrid.SelectedItem as ISProjectSchool.Models.Subjects;
            if (selectedSubject != null)
            {
                var idsubject = selectedSubject.SubjectID;
                try
                {
                    string query = @"
DELETE FROM [dbo].[Subjects]
WHERE SubjectID = @SubjectID";

                    using (SqlCommand command = new SqlCommand(query, DB.getConnection()))
                    {
                        command.Parameters.AddWithValue("@SubjectID", idsubject.ToString());

                        DB.openConnection();
                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Предмет успешно удален!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadSubjectsData();
                        }
                        else
                        {
                            MessageBox.Show("Предмет с таким ID не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                        DB.closeConnection();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
