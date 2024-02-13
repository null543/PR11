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
    /// Логика взаимодействия для LessonPanel.xaml
    /// </summary>
    public partial class LessonPanel : Window
    {
        private string scheduleID;
        public LessonPanel(string scheduleId)
        {
            InitializeComponent();
            this.scheduleID = scheduleId;
        }
    }
}
