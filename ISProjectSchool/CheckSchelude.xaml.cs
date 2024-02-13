using ISProjectSchool.DataBase;
using ISProjectSchool.Models;
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
    /// Логика взаимодействия для CheckSchelude.xaml
    /// </summary>
    public partial class CheckSchelude : Window
    {
        public CheckSchelude()
        {
            InitializeComponent();
            LoadScheduleData();
        }

        private void LoadScheduleData()
        {
            var scheduleData = DatabaseHelper.LoadScheduleData(); // Предполагается, что этот метод возвращает List<ScheduleItem>
            ScheduleDataGrid.ItemsSource = scheduleData;
        }
    }
}
