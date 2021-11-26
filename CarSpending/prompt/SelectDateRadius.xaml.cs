using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
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
    /// Логика взаимодействия для SelectDateRadius.xaml
    /// </summary>
    public partial class SelectDateRadius : Window
    {
        private MainWindow mainWindow;
        private Run dateRadius_exp, countNote_exp;
        private ListBox listOfExpenses, listOfRefills, listOfService;
        private TextBlock totalExpesns_exp, dayExpesns_exp, totalMileage_exp, dayMilage_exp;

        public SelectDateRadius(Run dateRadius_exp,ListBox listOfExpenses,Run countNote_exp, TextBlock totalExpesns_exp, TextBlock dayExpesns_exp, TextBlock totalMileage_exp, TextBlock dayMilage_exp,ListBox listOfRefills, ListBox listOfService)
        {
            this.totalExpesns_exp = totalExpesns_exp;
            this.dayExpesns_exp = dayExpesns_exp;
            this.totalMileage_exp = totalMileage_exp;
            this.dayMilage_exp = dayMilage_exp;
            this.dateRadius_exp = dateRadius_exp;
            this.listOfExpenses = listOfExpenses;
            this.countNote_exp = countNote_exp;
            this.listOfRefills = listOfRefills;
            this.listOfService = listOfService;
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void addDateRadius(object sender, RoutedEventArgs e)
        {
            DataClass dataClass = new DataClass();
            mainWindow = new MainWindow();

            bool isCorect = true;
            string expenseDateStart = startDate.Text;
            string expenseDateFinish = finishtDate.Text;


            mainWindow.ValidationDate(startDate, ref expenseDateStart, ref isCorect);
            mainWindow.ValidationDate(finishtDate, ref expenseDateFinish, ref isCorect);

            var test = dataClass.selectQuery("select * from Expenses where Expense_date BETWEEN  '" + expenseDateStart +
                                             "' AND '" + expenseDateFinish + "'").Rows;

            var filtersDateList = dataClass.SelecExpenses(test);

            mainWindow.FilterExpense(ref listOfExpenses, new ObservableCollection<Expense>(), new List<Expense>(), filtersDateList,listOfRefills,listOfService);

            // listOfExpenses.ItemsSource = new ObservableCollection<Expense>(filtersDateList);// push items into listblock with expenses


            dateRadius_exp.Text = "(" + expenseDateStart.Replace("-", "/") + " - " +
                                  expenseDateFinish.Replace("-", "/") + ")";// show date raius


            List<Expense> tempList = new List<Expense>();

            if (listOfExpenses == listOfRefills)
            {
                foreach (var testItem in filtersDateList)
                {
                    if (testItem.Service_id == -1)
                    {
                        tempList.Add(testItem);
                    }
                }

            }
            else if (listOfExpenses == listOfService)
            {
                foreach (var testItem in filtersDateList)
                {
                    if (testItem.Refill_id == -1)
                    {
                        tempList.Add(testItem);
                    }
                }

            }
            else
            {
                tempList = filtersDateList;
            }

            countNote_exp.Text = tempList.Count+"";// show count notes 
            var calcutaeStatistick = countTotalCost(tempList);
            totalExpesns_exp.Text = calcutaeStatistick["totalCost"] + " км";
            dayExpesns_exp.Text = calcutaeStatistick["averageCost"] + " км";
            totalMileage_exp.Text = calcutaeStatistick["resultMileage"] + " ₴";
            dayMilage_exp.Text = calcutaeStatistick["averageMileage"]+ " ₴";
            Close();
            
        }

        public Dictionary<string, double> countTotalCost(List<Expense> filtersDateList)//get all values for general statistic
        {
            Dictionary<string, double> resultDictionary = new Dictionary<string, double>();
            double result = 0;
            double resultMileage = 0;
            foreach (Expense itemExpense in filtersDateList)
            {
                result += itemExpense.TotalCost;
                resultMileage += itemExpense.Mileage_num;
            }
            resultDictionary.Add("totalCost",result);
            resultDictionary.Add("averageCost", result/ filtersDateList.Count);
            resultDictionary.Add("resultMileage", resultMileage);
            resultDictionary.Add("averageMileage", resultMileage / filtersDateList.Count);


            return resultDictionary;
        }

    }
}
