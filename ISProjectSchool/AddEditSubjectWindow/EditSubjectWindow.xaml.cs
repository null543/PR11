using ISProjectSchool.DataBase;
using System;
using System.Collections.Generic;
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

    
    
    public partial class EditSubjectWindow : Window
    {
        DatabaseHelper DB = new DatabaseHelper();
        private string subjectName;
        private int subjectID;
        public EditSubjectWindow(int subjectId, string subjectname)
        {
            InitializeComponent();
            subjectName = subjectname;
            subjectID = subjectId;
            txtSubjectName.Text = subjectName;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSubjectName.Text))
            {
                MessageBox.Show("Заполните все поля", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                try
                {
                    string query = @"
UPDATE [dbo].[Subjects]
SET SubjectName = @SubjectName
WHERE SubjectID = @SubjectID";

                    using (SqlCommand command = new SqlCommand(query, DB.getConnection()))
                    {
                        command.Parameters.AddWithValue("@SubjectName", txtSubjectName.Text);
                        command.Parameters.AddWithValue("@SubjectID", subjectID.ToString());

                        DB.openConnection();
                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Данные успешно обновлены!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Предмет с таким ID не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                        DB.closeConnection();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                this.Close();
            }
        }

    }
}
