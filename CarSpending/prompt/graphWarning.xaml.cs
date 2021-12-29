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

namespace CarSpending.prompt
{
    /// <summary>
    /// Логика взаимодействия для graphWarning.xaml
    /// </summary>
    public partial class graphWarning : Window
    {
        public graphWarning()
        {
            InitializeComponent();
        }

        private void CancelDeleteExpense_click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
