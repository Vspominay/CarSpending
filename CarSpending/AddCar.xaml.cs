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

namespace CarSpending
{
    /// <summary>
    /// Логика взаимодействия для AddCar.xaml
    /// </summary>
    public partial class AddCar : Window
    {
        private User user;
        private ApplicationContext db;

        public AddCar(User user)
        {
            InitializeComponent();
            this.user = user;
            db = new ApplicationContext();
        }

        private void addCarClose_click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddAuto_click(object sender, RoutedEventArgs e)// click on the button for add auto
        {
            string carBrand = CarBrand.Text.Trim();
            string carVIN = CarVIN.Text.Trim().ToUpper();
            double carMileage = 0;
            double carTankVolume = 0;

            string patternText = @"[-!#\$%&'\*\+/=\?\@]";
            string patternVIN = @"[A-HJ-NPR-Z0-9]{13}[0-9]{4}";// pattern for check VIN number

            bool isCorect = true;// checkvalue for correct data

            if (carBrand.Length < 3 || Regex.IsMatch(carBrand, patternText, RegexOptions.IgnoreCase))// check car band
            {
                CarBrand.Foreground = Brushes.DarkRed;
                TextFieldAssist.SetUnderlineBrush(CarBrand, Brushes.DarkRed);
                HintAssist.SetHint(CarBrand, "Некоректно заполненое поле ");
                isCorect = false;
            }

            if ((carVIN.Length != 17 && carVIN.Length != 0) || (!Regex.IsMatch(carVIN, patternVIN, RegexOptions.IgnoreCase) && carVIN.Length != 0))// check vin number
            {
                CarVIN.Foreground = Brushes.DarkRed;
                TextFieldAssist.SetUnderlineBrush(CarVIN, Brushes.DarkRed);
                HintAssist.SetHint(CarVIN, "Некоректно заполненое поле ");
                isCorect = false;

            }

            if (CarMileage.Text.Length <= 0 || !Regex.IsMatch(CarMileage.Text, @"^[0-9,.]*$"))// check car mileage
            {
                CarMileage.Foreground = Brushes.DarkRed;
                TextFieldAssist.SetUnderlineBrush(CarMileage, Brushes.DarkRed);
                HintAssist.SetHint(CarMileage, "Некоректно заполненое поле ");
                isCorect = false;
            }
            else
            {
                if (CarMileage.Text.Trim().Contains('.'))
                {
                    carMileage = Convert.ToDouble(CarMileage.Text.Trim().Replace('.', ','));
                }
                else
                {
                    carMileage = Convert.ToDouble(CarMileage.Text.Trim());
                }
            }

            if (CarTankVolume.Text.Length <= 0 || !Regex.IsMatch(CarTankVolume.Text, @"^[0-9,.]*$"))// check car tank
            {
                CarTankVolume.Foreground = Brushes.DarkRed;
                TextFieldAssist.SetUnderlineBrush(CarTankVolume, Brushes.DarkRed);
                HintAssist.SetHint(CarTankVolume, "Некоректные данные");
                isCorect = false;
            }
            else
            {
                if (CarTankVolume.Text.Trim().Contains('.'))
                {
                    carTankVolume = Convert.ToDouble(CarTankVolume.Text.Trim().Replace('.', ','));
                }
                else
                {
                    carTankVolume = Convert.ToDouble(CarTankVolume.Text.Trim());
                }
            }


            if (isCorect)
            {
                Car newCar = new Car(carMileage,carTankVolume,user.User_id,carBrand, carVIN);

                db.Cars.Add(newCar);
                db.SaveChanges();

                MainWindow mainWindow = new MainWindow(user);
                this.Owner.Close();
                mainWindow.Show();
                Close();

            }
        }
    }
}
