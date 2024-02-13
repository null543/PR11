using ISProjectSchool.DataBase;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Net;
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
using System.Collections.ObjectModel;
using ISProjectSchool.Models;
using System.Data.Common;

namespace ISProjectSchool.AddEditTeacherWindow
{
    /// <summary>
    /// Логика взаимодействия для AddTeacherWindow.xaml
    /// </summary>
    public partial class AddTeacherWindow : Window
    {
        DatabaseHelper DB = new DatabaseHelper();
        Groups gr = new Groups();
        public AddTeacherWindow()
        {
            InitializeComponent();
            LoadGroups();
            LoadSubjects();
        }

        public ObservableCollection<Groups> Groups { get; set; }
        public void LoadGroups()
        {
            Groups = new ObservableCollection<Groups>();
           
            string query = "SELECT GroupID, GroupName FROM [dbo].[Groups]";

           
                SqlCommand cmd = new SqlCommand(query, DB.getConnection());

                DB.openConnection();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Groups.Add(new Groups
                        {
                            GroupID = (int)reader["GroupID"],
                            GroupName = reader["GroupName"].ToString()
                        });
                    }
                    reader.Close();
                }
                DB.closeConnection();
            

            listBoxGroups.ItemsSource = Groups; // Привязка коллекции к ListBox
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

        //получение ID выбранных элементов из листа с гурппами
        public List<int> GetSelectedGroupIDs()
        {
            List<int> selectedGroupIDs = new List<int>();

            foreach (Groups selectedGroup in listBoxGroups.SelectedItems)
            {
                selectedGroupIDs.Add(selectedGroup.GroupID);
            }

            return selectedGroupIDs;
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
            this.Close();
        }

        private bool IsValidEmail(string email)
        {
            // Простая проверка на соответствие шаблону электронной почты
            string pattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
            return Regex.IsMatch(email, pattern);
        }



        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка на заполненность и валидность почты
                if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                    string.IsNullOrWhiteSpace(txtLastName.Text) ||
                    string.IsNullOrWhiteSpace(txtMiddleName.Text) ||
                    string.IsNullOrWhiteSpace(txtLogin.Text) ||
                    listBoxGroups.SelectedItem == null ||
                    !IsValidEmail(txtLogin.Text)) // Используем функцию проверки почты
                {
                    MessageBox.Show("Все поля должны быть заполнены и адрес электронной почты должен быть действительным.", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }




                // Генерация пароля
                string password = GeneratePassword();
                List<int> idgroup = GetSelectedGroupIDs();
                List<int> idsubject = GetSelectedSubjectsIDs();
                int roleID = 2; // Указано, что RoleID всегда равен 1 для студентов

                // Запись в базу данных

                string query = @"
insert into Users (FirstName, LastName, MiddleName, RoleID, Email, Password)
values
(@FirstName,@LastName,@MiddleName,@RoleID,@Login,@Password)
";

                using (SqlCommand command = new SqlCommand(query, DB.getConnection()))
                {
                    command.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                    command.Parameters.AddWithValue("@LastName", txtLastName.Text);
                    command.Parameters.AddWithValue("@MiddleName", txtMiddleName.Text);
                    command.Parameters.AddWithValue("@Login", txtLogin.Text);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@RoleID", roleID);
                    DB.openConnection();
                    int result = command.ExecuteNonQuery();
                    
                }

                string queryGetTeacherID = "SELECT @@IDENTITY AS TeacherID";
                int teacherId;
                using (SqlCommand cmd = new SqlCommand(queryGetTeacherID, DB.getConnection()))
                {
                    teacherId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Вставка записей в TeacherGroups для каждой группы из списка idgroup
                string queryTeacherGroupInsert = "INSERT INTO TeacherGroups (TeacherID, GroupID) VALUES (@TeacherID, @GroupID)";
                using (SqlCommand cmd = new SqlCommand(queryTeacherGroupInsert, DB.getConnection()))
                {
                    cmd.Parameters.AddWithValue("@TeacherID", teacherId);

                    foreach (int groupId in idgroup)
                    {
                        cmd.Parameters.AddWithValue("@GroupID", groupId);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.RemoveAt("@GroupID"); // Удаление параметра для следующей итерации
                    }
                }

                string queryTeacherSubjectsInsert = "INSERT INTO SubjectTeacher (TeacherID, SubjectID) VALUES (@TeacherID, @SubjectID)";
                using (SqlCommand cmd = new SqlCommand(queryTeacherSubjectsInsert, DB.getConnection()))
                {
                    cmd.Parameters.AddWithValue("@TeacherID", teacherId);

                    foreach (int subjectid in idsubject)
                    {
                        cmd.Parameters.AddWithValue("@SubjectID", subjectid);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.RemoveAt("@SubjectID"); // Удаление параметра для следующей итерации
                    }
                }

                DB.closeConnection();
                MessageBox.Show("Преподователь был успешно добавлен!","Уведомление",MessageBoxButton.OK, MessageBoxImage.Information);
                SendEmailPassword(txtLogin.Text, password, txtFirstName.Text, txtMiddleName.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка, обратитесь к в техническую поддержку \n{ex}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void SendEmailPassword(string email, string password, string firstname, string middlename)
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
            mail.Subject = "Вас успешно зарегистрировали в системе";
            mail.Body = $"Здравствуйте {firstname} {middlename}, вас успешно зарегистрировали в образовательной системе School Factory в качестве преподователя<br>" +
 $"Для входа используйте следующие данные:<br>" +
 $"Логин: {email}<br>" +
 $"Пароль: {password}<br>";
            Client.Send(mail);


        }

        // Метод для генерации пароля
        private string GeneratePassword()
        {
            int length = 8; // Можно изменить на желаемую длину пароля

            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_~-";
            Random random = new Random();
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[random.Next(validChars.Length)];
            }
            return new string(chars);
        }
    }
}
