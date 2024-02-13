using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using ISProjectSchool.DataBase;
using System.Text.RegularExpressions;

namespace ISProjectSchool.AddWindow
{
    /// <summary>
    /// Логика взаимодействия для AddStudentWindow.xaml
    /// </summary>
    public partial class AddStudentWindow : Window
    {
        DatabaseHelper DB = new DatabaseHelper();
        
        public AddStudentWindow()
        {
            
            InitializeComponent();
            DB.openConnection();
            ComboBoxFiller cmbGroups = new ComboBoxFiller(DB.getConnection(), "select [GroupID], [GroupName]  from [dbo].[Groups]");
            cmbGroups.FillComboBox(cmbGroup, "GroupName", "GroupID");
            DB.closeConnection();

        }
        private bool IsValidEmail(string email)
        {
            // Простая проверка на соответствие шаблону электронной почты
            string pattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
            return Regex.IsMatch(email, pattern);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка на заполненность и валидность почты
                if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                    string.IsNullOrWhiteSpace(txtLastName.Text) ||
                    string.IsNullOrWhiteSpace(txtMiddleName.Text) ||
                    string.IsNullOrWhiteSpace(txtLogin.Text) ||
                    cmbGroup.SelectedItem == null ||
                    !IsValidEmail(txtLogin.Text)) // Используем функцию проверки почты
                {
                    MessageBox.Show("Все поля должны быть заполнены и адрес электронной почты должен быть действительным.","Внимание!",MessageBoxButton.OK,MessageBoxImage.Warning);
                    return;
                }


                string groupId = ((ComboBoxItem)cmbGroup.SelectedItem).Tag.ToString();


                // Генерация пароля
                string password = GeneratePassword();
                int roleID = 1; // Указано, что RoleID всегда равен 1 для студентов

                // Запись в базу данных

                string query = @"
DECLARE @UserID INT, @StudentID INT

EXECUTE AddStudentUser
    @FirstName,
    @LastName,
    @MiddleName,
    @Login,
    @PasswordHash,
    @GroupID;
";

                using (SqlCommand command = new SqlCommand(query, DB.getConnection()))
                {
                    command.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                    command.Parameters.AddWithValue("@LastName", txtLastName.Text);
                    command.Parameters.AddWithValue("@MiddleName", txtMiddleName.Text);
                    command.Parameters.AddWithValue("@Login", txtLogin.Text);
                    command.Parameters.AddWithValue("@PasswordHash", password);
                    command.Parameters.AddWithValue("@RoleID", roleID);
                    command.Parameters.AddWithValue("@GroupID", groupId);

                    DB.openConnection();
                    int result = command.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Студент успешно добавлен.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                        DB.closeConnection();
                        SendEmailPassword(txtLogin.Text, password, txtFirstName.Text, txtMiddleName.Text);


                    }
                    else
                    {
                        MessageBox.Show("Ошибка при добавлении студента.");
                        DB.closeConnection();

                    }
                    DB.closeConnection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка, обратитесь к в техническую поддержку \n{ex}","Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
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
            mail.Body = $"Здравствуйте {firstname} {middlename}, вас успешно зарегистрировали в образовательной системе School Factory<br>" +
 $"Для входа используйте следующие данные:<br>" +
 $"Логин: {email}<br>" +
 $"Пароль: {password}";
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
