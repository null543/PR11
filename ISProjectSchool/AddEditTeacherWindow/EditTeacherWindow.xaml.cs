using ISProjectSchool.Classes;
using ISProjectSchool.DataBase;
using ISProjectSchool.MainContentWindows;
using ISProjectSchool.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ISProjectSchool.AddEditTeacherWindow
{
    /// <summary>
    /// Логика взаимодействия для EditTeacherWindow.xaml
    /// </summary
    /// 


    public partial class EditTeacherWindow : Window
    {
        DatabaseHelper DB = new DatabaseHelper();
        // Поля класса
        private int iduser;
        private string name;
        private string surname;
        private string middlename;
        private string login;
        private string groups;
        private string subjects;
        public EditTeacherWindow(int iduser, string name, string surname, string middlename, string login, string groups, string subjects)
        {
            InitializeComponent();
            LoadSubjects();
            LoadGroups();

            // Инициализация полей класса значениями из конструктора
            this.iduser = iduser;
            this.name = name;
            this.surname = surname;
            this.middlename = middlename;
            this.login = login;
            this.groups = groups;
            this.subjects = subjects;

            Console.WriteLine("ID пользователя: " + iduser);
            Console.WriteLine("Имя: " + name);
            Console.WriteLine("Фамилия: " + surname);
            Console.WriteLine("Отчество: " + middlename);
            Console.WriteLine("Логин: " + login);
            Console.WriteLine("название группы: " + groups);
            Console.WriteLine("название предмета: " + subjects);

            txtFirstName.Text = name;
            txtLastName.Text = surname;
            txtMiddleName.Text = middlename;
            txtLogin.Text = login;




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


                // Предположим, что у вас есть обновленные данные пользователя, списки обновленных групп и предметов
                Dictionary<string, string> updatedUserData = new Dictionary<string, string>
{
    {"FirstName", txtFirstName.Text},
    {"LastName", txtLastName.Text},
    {"MiddleName", txtMiddleName.Text},
                    {"Email", txtLogin.Text},
    // Другие обновленные поля
};

                List<int> idgroup = GetSelectedGroupIDs();
                List<int> idsubject = GetSelectedSubjectsIDs();

                // Вызов метода EditUserAndRelatedData
                DB.EditUserAndRelatedData(iduser.ToString(), updatedUserData, idgroup, idsubject);
                MessageBox.Show("Данные успешно обновлены!","Информация",MessageBoxButton.OK,MessageBoxImage.Information);
                if (txtLogin.Text != login)
                {
                    SendEmailChangeEmail(login, txtFirstName.Text, txtMiddleName.Text, txtLogin.Text);
                    MessageBox.Show("Студенту отправлено сообщение о смене почты", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                this.Close();




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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

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

        private void ListBoxItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as ListBoxItem);

            if (item != null && item.IsSelected)
            {
                // Снимаем выделение, если элемент уже был выделен
                listBoxGroups.SelectedItems.Remove(item.Content);
            }
            else
            {
                // Выделяем элемент
                listBoxGroups.SelectedItems.Add(item.Content);
            }
        }
        private void ListBoxItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox == null) return;

            // Получаем элемент, на который был совершен клик
            var clickedElement = e.OriginalSource as FrameworkElement;
            if (clickedElement == null) return;

            // Получаем DataContext этого элемента, который должен быть элементом списка
            var item = clickedElement.DataContext;

            listBox.SelectionMode = SelectionMode.Multiple;

            // Переключаем выделение элемента
            if (listBox.SelectedItems.Contains(item))
            {
                listBox.SelectedItems.Remove(item);
            }
            else
            {
                listBox.SelectedItems.Add(item);
            }

            e.Handled = true;
        }





    }
}
