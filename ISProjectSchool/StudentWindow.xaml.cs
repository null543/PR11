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
    /// Логика взаимодействия для StudentWindow.xaml
    /// </summary>
    public partial class StudentWindow : Window
    {
        private string username;
        public StudentWindow(string username)
        {
            this.username = username;
            InitializeComponent();
            helloStudent.Text = "Добро пожаловать, " + username;

        }

        private void MyGrades_Click(object sender, RoutedEventArgs e)
        {
          
        }

        private void MySchedule_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
