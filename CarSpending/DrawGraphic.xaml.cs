using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CarSpending;
using LiveCharts;
using System;
using CarSpending.prompt;

namespace Wpf.CartesianChart.SolidColorChart
{
    /// <summary>
    /// Interaction logic for SolidColorExample.xaml
    /// </summary>
    public partial class SolidColorExample : Window
    {
        private ApplicationContext db;
        private Car car;

        public SolidColorExample(Car car,ref bool warning)
        {

            this.car = car;
            db = new ApplicationContext();
            InitializeComponent();

            if (db.Automations != null)
            {
                var dataGraph = db.Automations.ToList().FirstOrDefault(aut => aut.Car_id == car.Car_id)?.AutData.Split(' ');
                List<double> test = new List<double>();
                if (dataGraph == null)
                {
                    warning = true;
                    graphWarning wr = new graphWarning();
                    wr.ShowDialog();
                    return;
                }

                foreach (var itemArr in dataGraph)
                {
                    test.Add(Convert.ToDouble(itemArr));
                }

                Values = new ChartValues<double>(test);
                DataContext = this;
            }


        }

        public ChartValues<double> Values { get; set; }

        private void UpdateOnclick(object sender, RoutedEventArgs e)
        {
            Chart.Update(true);
        }
    }
}