using ISProjectSchool.DataBase;
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

namespace ISProjectSchool.AddEditStudentWindow
{
    /// <summary>
    /// Логика взаимодействия для EditStudentWindow.xaml
    /// </summary>
    public partial class EditStudentWindow : Window
    {
        DatabaseHelper DB = new DatabaseHelper();
        // Поля класса
        private int iduser;
        private string name;
        private string surname;
        private string middlename;
        private string login;
        private string group;

        // Конструктор класса
        public EditStudentWindow(int iduser, string name, string surname, string middlename, string login, string group)
        {
            InitializeComponent();
            // Инициализация полей класса значениями из конструктора
            this.iduser = iduser;
            this.name = name;
            this.surname = surname;
            this.middlename = middlename;
            this.login = login;
            this.group = group;
            DB.openConnection();
            ComboBoxFiller cmbGroups = new ComboBoxFiller(DB.getConnection(), "select [GroupID], [GroupName]  from [dbo].[Groups]");
            cmbGroups.FillComboBox(cmbGroup, "GroupName", "GroupID");
            foreach (var item in cmbGroup.Items)
            {
                string itemGenre = ((ComboBoxItem)item).Content.ToString();
                if (itemGenre == group)
                {
                    cmbGroup.SelectedItem = item;
                    break;
                }
            }

            txtFirstName1.Text = name;
            txtLastName.Text = surname;
            txtMiddleName.Text = middlename;
            txtLogin.Text = login;
            string groupId = ((ComboBoxItem)cmbGroup.SelectedItem).Tag.ToString();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Те же проверки на заполненность и валидность данных

                string groupId = ((ComboBoxItem)cmbGroup.SelectedItem).Tag.ToString();

                // Если нужно изменить пароль, раскомментируйте следующую строку
                // string password = GeneratePassword();

                // SQL-запрос на обновление данных студента
                string query = @"
DECLARE @UserID INT, @StudentID INT

EXECUTE UpdateStudentUser
    @UserIDD,
    @FirstName, 
    @LastName,
    @MiddleName,
    @Login,
    @GroupID;
";

                using (SqlCommand command = new SqlCommand(query, DB.getConnection()))
                {
                    command.Parameters.AddWithValue("@UserIDD", iduser); // Использование существующего идентификатора
                    command.Parameters.AddWithValue("@FirstName", txtFirstName1.Text);
                    command.Parameters.AddWithValue("@LastName", txtLastName.Text);
                    command.Parameters.AddWithValue("@MiddleName", txtMiddleName.Text);
                    command.Parameters.AddWithValue("@Login", txtLogin.Text);
                    command.Parameters.AddWithValue("@GroupID", groupId);

                    DB.openConnection();
                    int result = command.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Данные студента успешно обновлены.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                        if (txtLogin.Text != login)
                        {
                            SendEmailChangeEmail(login, txtFirstName1.Text, txtMiddleName.Text, txtLogin.Text);
                            MessageBox.Show("Студенту отправлено сообщение о смене почты","Уведомление",MessageBoxButton.OK,MessageBoxImage.Information);

                        }
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при обновлении данных студента.");
                    }
                    DB.closeConnection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SendEmailChangeEmail(string oldemail, string firstname, string middlename, string newemail)
        {
            SmtpClient Client = new SmtpClient();
            Client.Credentials = new NetworkCredential("theatre.send.code@mail.ru", "60erfdJyeahNv0seCKHZ");
            Client.Host = "smtp.mail.ru";
            Client.Port = 587;
            Client.EnableSsl = true;
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("theatre.send.code@mail.ru");
            mail.To.Add(new MailAddress(oldemail));
            mail.IsBodyHtml = true;
            mail.Subject = "Смена почты администратором";
            mail.Body = $"<p>Здравствуйте {firstname} {middlename}, администратор обновил почту вашего аккаунта на {newemail}</p>\r\n<p>Дальнейшая работа будет по новой почте, если произошла ошибка, то обратитесь к администрации школы.</p>";
            Client.Send(mail);


            // Второе письмо
            MailMessage mail2 = new MailMessage();
            mail2.From = new MailAddress("theatre.send.code@mail.ru");
            mail2.To.Add(new MailAddress(newemail));
            mail2.IsBodyHtml = true;
            mail2.Subject = "Смена почты администратором";
            mail2.Body = $"<p>Здравствуйте {firstname} {middlename}, администратор обновил почту вашего аккаунта с {oldemail} на {newemail}</p>\r\n<p>Дальнейшая работа будет по новой почте, если произошла ошибка, то обратитесь к администрации школы.</p>";
            Client.Send(mail2);





        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
