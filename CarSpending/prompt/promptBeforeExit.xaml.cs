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
    /// Логика взаимодействия для promptBeforeExit.xaml
    /// </summary>
    public partial class promptBeforeExit : Window
    {
        public promptBeforeExit()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Close();
        }

        private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_PreviewMouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}
