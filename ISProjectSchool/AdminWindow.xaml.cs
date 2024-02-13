using ISProjectSchool.Classes;
using ISProjectSchool.MainContentWindows;
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
using System.Windows.Shapes;


namespace ISProjectSchool
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        private User currentUser;
        public AdminWindow(User user)
        {
            InitializeComponent();
            SizeChanged += Window_SizeChanged;
            currentUser = user;

        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Вычисляем новый размер шрифта на основе размера окна
            double scaleFactor = Math.Min(e.NewSize.Width / 800, e.NewSize.Height / 600);

            // Применяем новый размер шрифта к кнопкам
            foreach (UIElement element in ButtonContainer.Children)
            {
                if (element is Button button)
                {
                    button.FontSize = 12 * scaleFactor;
                }
            }
        }




        private void StudentsButton_Click(object sender, RoutedEventArgs e)
        {
            StudentsManagementUserControl studentsControl = new StudentsManagementUserControl();

            // Показать UserControl для управления студентами
            MainContent.Content = studentsControl;

            txtPanel.Text = "Панель Администратора -> Студенты";
        }

        private void TeachersButton_Click(object sender, RoutedEventArgs e)
        {
            TeachersManagementUserControl teachersManagementUserControl = new TeachersManagementUserControl();
            // Показать UserControl для управления преподавателями
            MainContent.Content = teachersManagementUserControl ;

            txtPanel.Text = "Панель Администратора -> Преподователи";
        }

        private void SubjectsButton_Click(object sender, RoutedEventArgs e)
        {
            SubjectsManagementUserControl subjectsManagementUserControl = new SubjectsManagementUserControl();
            // Показать UserControl для управления преподавателями
            MainContent.Content = subjectsManagementUserControl;

            txtPanel.Text = "Панель Администратора -> Предметы";
        }

        private void GroupsButton_Click(object sender, RoutedEventArgs e)
        {
            GroupsManagementUserControl groupsManagementUserControl = new GroupsManagementUserControl();
            // Показать UserControl для управления преподавателями
            MainContent.Content = groupsManagementUserControl;

            txtPanel.Text = "Панель Администратора -> Группы";
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            Schedules schedules = new Schedules();
            schedules.ShowDialog();
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            CheckSchelude checkSchelude = new CheckSchelude();
            checkSchelude.ShowDialog();
        }
    }
}
