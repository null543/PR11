using ISProjectSchool.AddEditGroupWindow;
using ISProjectSchool.AddEditTeacherWindow;
using ISProjectSchool.DataBase;
using ISProjectSchool.Models;
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

namespace ISProjectSchool.MainContentWindows
{
    /// <summary>
    /// Логика взаимодействия для GroupsManagementUserControl.xaml
    /// </summary>
    public partial class GroupsManagementUserControl : UserControl
    {
        DatabaseHelper DB = new DatabaseHelper();
        public GroupsManagementUserControl()
        {
            InitializeComponent();
            LoadGroupsData();
        }

        private void LoadGroupsData()
        {
            List<Groups> groups = DatabaseHelper.LoadGroupsData();
            StudentsDataGrid.ItemsSource = groups;
        }

        private void AddGroupButton_Click(object sender, RoutedEventArgs e)
        {
            AddGroupWindow addGroupWindow = new AddGroupWindow();

            // Отобразить окно и ожидать его закрытия
            bool? dialogResult = addGroupWindow.ShowDialog();

            // Проверить результат диалога (например, была ли нажата кнопка "ОК" или "Отмена")
            if (dialogResult == true)
            {
                LoadGroupsData();
            }
            else
            {

                LoadGroupsData(); 
            }
        }

        private void DeleteGroupButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedGroup = StudentsDataGrid.SelectedItem as ISProjectSchool.Models.Groups;
            if (selectedGroup != null)
            {
                var selectedGroupId = selectedGroup.GroupID;
                try
                {
                    DB.openConnection();


                    string deleteGroupSubjectsQuery = "DELETE FROM [dbo].[GroupSubjects] WHERE GroupID = @GroupID";
                    SqlCommand command = new SqlCommand(deleteGroupSubjectsQuery, DB.getConnection());
                    command.Parameters.AddWithValue("@GroupID", selectedGroupId);
                    command.ExecuteNonQuery();


                    string deleteTeachersGroupQuery = "DELETE FROM [dbo].[TeacherGroups] WHERE GroupID = @GroupID";
                    command = new SqlCommand(deleteTeachersGroupQuery, DB.getConnection());
                    command.Parameters.AddWithValue("@GroupID", selectedGroupId);
                    int result = command.ExecuteNonQuery();
                    command.ExecuteNonQuery();

                    string deleteGroupQuery = "DELETE FROM [dbo].[Groups] WHERE GroupID = @GroupID";
                    command = new SqlCommand(deleteGroupQuery, DB.getConnection());
                    command.Parameters.AddWithValue("@GroupID", selectedGroupId);
                    command.ExecuteNonQuery();

                  

                    
                    MessageBox.Show("Группа успешно удалена!\n\nСтуденты которые находятся в этой группе были перемещены в архив!", "Информация",MessageBoxButton.OK,MessageBoxImage.Information);
                    
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Неизвестная ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    DB.closeConnection();
                }

            }
        }

        private void EditGroupButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedGroup = StudentsDataGrid.SelectedItem as ISProjectSchool.Models.Groups;
            if (selectedGroup != null)
            {
                var idgroup = selectedGroup.GroupID;

                EditGroupWindow editGroupWindow = new EditGroupWindow(idgroup);

                // Отобразить окно и ожидать его закрытия
                bool? dialogResult = editGroupWindow.ShowDialog();

                // Проверить результат диалога (например, была ли нажата кнопка "ОК" или "Отмена")
                if (dialogResult == true)
                {
                    LoadGroupsData();
                }
                else
                {

                    LoadGroupsData();
                }
            }
        }
    }
}
