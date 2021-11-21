using System;
using System.Collections.Generic;
using System.Linq;
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
using MaterialDesignThemes.Wpf;

namespace CarSpending.prompt
{
    /// <summary>
    /// Логика взаимодействия для AddServiceType.xaml
    /// </summary>
    public partial class AddServiceType : Window
    {
        private ApplicationContext db;
        private User user;


        public AddServiceType(User user)
        {
            InitializeComponent();
            db = new ApplicationContext();
            this.user = user;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonBaseAdd_OnClick(object sender, RoutedEventArgs e)
        {
            string favorName = inputNewType_service.Text.Trim();
            string patternText = @"[-!#\$%&'\*\+/=\?\@]";


            if (favorName.Length < 3 || Regex.IsMatch(favorName, patternText, RegexOptions.IgnoreCase))
            {
                inputNewType_service.Foreground = Brushes.DarkRed;
                TextFieldAssist.SetUnderlineBrush(inputNewType_service, Brushes.DarkRed);
                HintAssist.SetHint(inputNewType_service, "Некоректно заполненое поле ");
            }
            else
            {
                favorType newFavor = new favorType(favorName);
                db.FavorTypes.Add(newFavor);
                db.SaveChanges();

                MainWindow mainWindow = new MainWindow(user);
                this.Owner.Close();
                mainWindow.Show();
                Close();
            }

           
        }
    }
}
