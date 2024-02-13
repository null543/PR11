using ISProjectSchool.DataBase;
using ISProjectSchool.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace ISProjectSchool.AddEditSubjectWindow
{
    /// <summary>
    /// Логика взаимодействия для AddSubjectWindow.xaml
    /// </summary>
    public partial class AddSubjectWindow : Window
    {
        DatabaseHelper DB = new DatabaseHelper();
        public AddSubjectWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtGroupName.Text))
            {
                MessageBox.Show("Заполните все поля", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                string query = @"
INSERT INTO [dbo].[Subjects] (SubjectName) VALUES (@SubjectName)";
                using (SqlCommand command = new SqlCommand(query, DB.getConnection()))
                {
                    command.Parameters.AddWithValue("@SubjectName", txtGroupName.Text);
                    DB.openConnection();
                    command.ExecuteScalar();

                    DB.closeConnection();
                }

                MessageBox.Show("Данные успешно добавлены!","Уведомление",MessageBoxButton.OK,MessageBoxImage.Information);
                this.Close();
            }
        }
    }
}
