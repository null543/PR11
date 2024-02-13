using ISProjectSchool.AddEditTeacherWindow;
using ISProjectSchool.DataBase;
using ISProjectSchool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
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
using ISProjectSchool.AddEditStudentWindow;

namespace ISProjectSchool.MainContentWindows
{
    /// <summary>
    /// Логика взаимодействия для TeachersManagementUserControl.xaml
    /// </summary>
    public partial class TeachersManagementUserControl : UserControl
    {
        DatabaseHelper DB = new DatabaseHelper();
        public TeachersManagementUserControl()
        {
            InitializeComponent();
            LoadStudentsData();
        }

        public void LoadStudentsData()
        {
            List<Teachers> students = DatabaseHelper.LoadTeachersData();
            StudentsDataGrid.ItemsSource = students;
        }

        private void AddTeahcersButton_Click(object sender, RoutedEventArgs e)
        {
            AddTeacherWindow addTeacherWindow = new AddTeacherWindow();
            addTeacherWindow.Show();
        }

        private void DeleteTeachersButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedStudent = StudentsDataGrid.SelectedItem as ISProjectSchool.Models.Teachers;
            if (selectedStudent != null)
            {
                var iduser = selectedStudent.UserID.ToString();
                var userDetails = DB.GetUserDetails(iduser);
                string userEmail = userDetails.email;
                string userFirstName = userDetails.firstName;
                string userMiddleName = userDetails.middleName;
                string studentFullName = DB.GetStudentFullName(iduser);

                try
                {
                    // Вызов нового метода для удаления пользователя и связанных данных
                    DB.DeleteUserAndRelatedData(iduser);

                    
                        LoadStudentsData();
                        MessageBox.Show($"Преподователь {studentFullName} успешно удален", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        SendEmailPassword(userEmail, userFirstName, userMiddleName);
                    
                   
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Произошла ошибка при удалении: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SendEmailPassword(string email, string firstname, string middlename)
        {
            SmtpClient Client = new SmtpClient();
            Client.Credentials = new NetworkCredential("theatre.send.code@mail.ru", "60erfdJyeahNv0seCKHZ");
            Client.Host = "smtp.mail.ru";
            Client.Port = 587;
            Client.EnableSsl = true;
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("theatre.send.code@mail.ru");
            mail.To.Add(new MailAddress(email));
            mail.IsBodyHtml = true;
            mail.Subject = "Ваш аккаунт был удален администратором";
            mail.Body = $"<p>Здравствуйте {firstname} {middlename}, ваш аккаунт был удален администратором из образовательной системы School Factory</p> <p>Для уточнения причины удаления обратитесь к администрации школы.</p>";
            Client.Send(mail);


        }

        private void EditTeachersButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedStudent = StudentsDataGrid.SelectedItem as ISProjectSchool.Models.Teachers;
            if (selectedStudent != null)
            {
                var iduser = selectedStudent.UserID;
                var name = selectedStudent.FirstName;
                var surname = selectedStudent.LastName;
                var middlename = selectedStudent.MiddleName;
                var login = selectedStudent.Login;
                var group = selectedStudent.GroupsID;
                var subjects = selectedStudent.SubjectsID;
                var editTeacherWindow = new EditTeacherWindow(iduser, name, surname, middlename, login, group, subjects);

                // Отобразить окно и ожидать его закрытия
                bool? dialogResult = editTeacherWindow.ShowDialog();

                // Проверить результат диалога (например, была ли нажата кнопка "ОК" или "Отмена")
                if (dialogResult == true)
                {
                    LoadStudentsData();
                }
                else
                {

                    LoadStudentsData();
                }
            }
            else
            {
                MessageBox.Show("Равно нулл");
            }
        }
    }
}
