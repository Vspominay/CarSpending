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
    /// Логика взаимодействия для ChangeICarInfo.xaml
    /// </summary>
    public partial class ChangeICarInfo : Window
    {
        private Car userCar;
        private User user;
        private ApplicationContext db;

        public ChangeICarInfo(Car userCar, User user)
        {
            InitializeComponent();
            this.user = user;
            this.userCar = userCar;
            CarBrand_change.Text = userCar.CarBrand;
            CarVIN_change.Text = userCar.Vin_num;
            CarMileage_change.Text = userCar.Mileage_num + "";
            CarTankVolume_change.Text = userCar.TankVolume_num + "";
        }

        private void addCarClose_click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddAuto_click(object sender, RoutedEventArgs e)
        {
            db = new ApplicationContext();
            var carBrand = CarBrand_change.Text.Trim();
            var vin = CarVIN_change.Text.Trim();
            var mileage = 0.0;
            var tank = 0.0;

            string patternText = @"[-!#\$%&'\*\+/=\?\@]";
            string patternVIN = @"[A-HJ-NPR-Z0-9]{13}[0-9]{4}";// pattern for check VIN number

            bool isCorect = true;// checkvalue for correct data

            if (carBrand.Length < 3 || Regex.IsMatch(carBrand, patternText, RegexOptions.IgnoreCase))// check car band
            {
                CarBrand_change.Foreground = Brushes.DarkRed;
                TextFieldAssist.SetUnderlineBrush(CarBrand_change, Brushes.DarkRed);
                HintAssist.SetHint(CarBrand_change, "Некоректно заполненое поле ");
                isCorect = false;
            }

            if ((vin.Length != 17 && vin.Length != 0) || (!Regex.IsMatch(vin, patternVIN, RegexOptions.IgnoreCase) && vin.Length != 0))// check vin number
            {
                CarVIN_change.Foreground = Brushes.DarkRed;
                TextFieldAssist.SetUnderlineBrush(CarVIN_change, Brushes.DarkRed);
                HintAssist.SetHint(CarVIN_change, "Некоректно заполненое поле ");
                isCorect = false;

            }

            if (CarMileage_change.Text.Length <= 0 || !Regex.IsMatch(CarMileage_change.Text, @"^[0-9,.]*$"))// check car mileage
            {
                CarMileage_change.Foreground = Brushes.DarkRed;
                TextFieldAssist.SetUnderlineBrush(CarMileage_change, Brushes.DarkRed);
                HintAssist.SetHint(CarMileage_change, "Некоректно заполненое поле ");
                isCorect = false;
            }
            else
            {
                if (CarMileage_change.Text.Trim().Contains('.'))
                {
                    mileage = Convert.ToDouble(CarMileage_change.Text.Trim().Replace('.', ','));
                }
                else
                {
                    mileage = Convert.ToDouble(CarMileage_change.Text.Trim());
                }
            }

            if (CarTankVolume_change.Text.Length <= 0 || !Regex.IsMatch(CarTankVolume_change.Text, @"^[0-9,.]*$"))// check car tank
            {
                CarTankVolume_change.Foreground = Brushes.DarkRed;
                TextFieldAssist.SetUnderlineBrush(CarTankVolume_change, Brushes.DarkRed);
                HintAssist.SetHint(CarTankVolume_change, "Некоректные данные");
                isCorect = false;
            }
            else
            {
                if (CarTankVolume_change.Text.Trim().Contains('.'))
                {
                    tank = Convert.ToDouble(CarTankVolume_change.Text.Trim().Replace('.', ','));
                }
                else
                {
                    tank = Convert.ToDouble(CarTankVolume_change.Text.Trim());
                }
            }


            if (isCorect)
            {
                var car = db.Cars.ToList().FirstOrDefault(c => c.Car_id == userCar.Car_id);
                car.CarBrand = carBrand;
                car.Mileage_num = mileage;
                car.TankVolume_num = tank;
                car.Vin_num = vin;

                db.SaveChanges();

                MainWindow mainWindow = new MainWindow(user);
                this.Owner.Close();
                mainWindow.Show();
                Close();

            }
        }
    }
}
