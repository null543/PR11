using ISProjectSchool.DataBase;
using System;
using System.Collections.Generic;
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

namespace ISProjectSchool
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameTextBox.Text;
            var password = PasswordBox.Password; // Предполагается, что пароли хранятся в хэшированном виде

            var user = DatabaseHelper.AuthenticateUser(username, password);

            if (user != null)
            {
                // Авторизация прошла успешно, открытие соответствующего окна
                switch (user.RoleName)
                {
                    case "DBAdmin":
                        AdminWindow adminWindow = new AdminWindow(user);
                        adminWindow.Show();
                        this.Close();
                        break;
                    case "Teacher":
                        TeacherWindow teacherWindow = new TeacherWindow(username);
                        teacherWindow.Show();
                        this.Close();
                        break;
                    case "Student":
                        StudentWindow studentWindow = new StudentWindow(username);
                        studentWindow.Show();
                        break;
                    default:
                        MessageBox.Show("Произошла неизвестная ошибка, обратитесь к администрации школы","Неизвестная ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
                        break;
                }
            }
            else
            {
                // Ошибка авторизации, показать сообщение
                MessageBox.Show("Неверное имя пользователя или пароль.", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

      
    }
}
