using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CarSpending;
using LiveCharts;
using System;

namespace Wpf.CartesianChart.SolidColorChart
{
    /// <summary>
    /// Interaction logic for SolidColorExample.xaml
    /// </summary>
    public partial class SolidColorExample : Window
    {
        private ApplicationContext db;
        private Car car;
        public SolidColorExample(Car car)
        {
            this.car = car;
            db = new ApplicationContext();
            InitializeComponent();

            var dataGraph = db.Automations.ToList().FirstOrDefault(aut => aut.Car_id == car.Car_id).AutData.Split(' ');
            List<double> test = new List<double>();

            foreach (var itemArr in dataGraph)
            {
                test.Add(Convert.ToDouble(itemArr));
            }

            
            Values = new ChartValues<double>(test);

            DataContext = this;
        }

        public ChartValues<double> Values { get; set; }

        private void UpdateOnclick(object sender, RoutedEventArgs e)
        {
            Chart.Update(true);
        }
    }
}