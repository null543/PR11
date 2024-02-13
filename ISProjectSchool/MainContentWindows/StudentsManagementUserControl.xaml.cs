using ISProjectSchool.AddEditStudentWindow;
using ISProjectSchool.AddWindow;
using ISProjectSchool.DataBase;
using ISProjectSchool.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using ISProjectSchool.Classes;

namespace ISProjectSchool.MainContentWindows
{
    /// <summary>
    /// Логика взаимодействия для StudentsManagementUserControl.xaml
    /// </summary>
    public partial class StudentsManagementUserControl : UserControl
    {
        DatabaseHelper db = new DatabaseHelper();
        public StudentsManagementUserControl()
        {
            InitializeComponent();
            LoadStudentsData();
        }
        // Метод для загрузки и отображения списка студентов
        private void LoadStudentsData()
        {
            List<Student> students = DatabaseHelper.LoadStudentsData();
            StudentsDataGrid.ItemsSource = students;
        }

        private void AddStudentButton_Click(object sender, RoutedEventArgs e)
        {
            AddStudentWindow addStudentWindow = new AddStudentWindow();
            addStudentWindow.Show();
        }

        private void EditStudentButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedStudent = StudentsDataGrid.SelectedItem as ISProjectSchool.Models.Student;
            if (selectedStudent != null)
            {
                var iduser = selectedStudent.UserID;
                var name = selectedStudent.FirstName;
                var surname = selectedStudent.LastName;
                var middlename = selectedStudent.MiddleName;
                var login = selectedStudent.Login;
                var group = selectedStudent.GroupID;
                var editStudentWindow = new EditStudentWindow(iduser, name, surname, middlename, login, group);
                editStudentWindow.Show();
            }
            else
            {
                MessageBox.Show("Равно нулл");
            }

        }

        private void DeleteStudentButton_Click(object sender, RoutedEventArgs e)
        {
            

            var selectedStudent = StudentsDataGrid.SelectedItem as ISProjectSchool.Models.Student;
            if (selectedStudent != null)
            {
                

                var iduser = selectedStudent.UserID.ToString();

                var userDetails = db.GetUserDetails(iduser);

                // Распаковка кортежа в отдельные переменные
                string userEmail = userDetails.email;
                string userFirstName = userDetails.firstName;
                string userMiddleName = userDetails.middleName;

                string studentFullName = db.GetStudentFullName(iduser);
                try
                {
                    bool result = db.DeleteUserandStudent(iduser);
                    if (result==true)
                    {
                        LoadStudentsData();
                        MessageBox.Show($"Студент {studentFullName} успешно удален","Информация",MessageBoxButton.OK,MessageBoxImage.Information);
                        SendEmailPassword(userEmail, userFirstName, userMiddleName);
                    }
                    else
                    {
                        MessageBox.Show($"Не удалось удалить студента {studentFullName}", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Произошла ошибка при удалении: " + ex.Message,"Ошибка",MessageBoxButton.OK, MessageBoxImage.Error);
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
    }
}
