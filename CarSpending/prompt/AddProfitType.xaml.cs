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
    /// Логика взаимодействия для AddProfitType.xaml
    /// </summary>
    public partial class AddProfitType : Window
    {
        private ApplicationContext db;
        private User user;


        public AddProfitType(User user)
        {
            InitializeComponent();
            db = new ApplicationContext();
            this.user = user;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void addNewTypeProfit_click(object sender, RoutedEventArgs e)
        {
            string profitName = inputNewType_profit.Text.Trim();
            string patternText = @"[-!#\$%&'\*\+/=\?\@]";


            if (profitName.Length < 3 || Regex.IsMatch(profitName, patternText, RegexOptions.IgnoreCase))
            {
                inputNewType_profit.Foreground = Brushes.DarkRed;
                TextFieldAssist.SetUnderlineBrush(inputNewType_profit, Brushes.DarkRed);
                HintAssist.SetHint(inputNewType_profit, "Некоректно заполненое поле ");
            }
            else
            {
                ProfitType newProfit = new ProfitType(profitName);
                db.ProfitTypes.Add(newProfit);
                db.SaveChanges();

                MainWindow mainWindow = new MainWindow(user);
                this.Owner.Close();
                mainWindow.Show();
                Close();
            }

        }
    }
}
