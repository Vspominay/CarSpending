using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using CarSpending.ListboxItems;

namespace CarSpending.prompt
{
    /// <summary>
    /// Логика взаимодействия для MonthStatistic.xaml
    /// </summary>
    /// 
    public partial class MonthStatistic : Window
    {
        private DataClass dataClass;
        
        public MonthStatistic()
        {
            dataClass = new DataClass();
            InitializeComponent();
            MakeStaticstic();
        }

        public void MakeStaticstic()
        {
            var rowCollectionStatistiks = dataClass.selectQuery("select  strftime(\"%m-%Y\", Expense_date) as 'month-year', SUM(TotalCost) as Price from Expenses group by strftime(\"%m-%Y\", Expense_date)").Rows;

            List<MonthCost> filtersDateList = new List<MonthCost>();

            string monthName;
            double totalCost;
        
            for (int i = 0; i < rowCollectionStatistiks.Count; i++)
            {
                monthName = rowCollectionStatistiks[i].ItemArray[0].ToString();
                totalCost = Convert.ToDouble(rowCollectionStatistiks[i].ItemArray[1]);

                filtersDateList.Add(new MonthCost
                {
                    MonthName = monthName,
                    TotalCost = totalCost
                });
            }

            topCostList.ItemsSource = new ObservableCollection<MonthCost>(filtersDateList);
            topCostListRep.ItemsSource = new ObservableCollection<MonthCost>(filtersDateList);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void makeReport(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.IsEnabled = false;
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintVisual(reportMontRep, "Общий отчёт за период");
                }
            }
            finally
            {
                this.IsEnabled = true;
            }
        }
    }
}
