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
        private ListBox listOfExpenses, listOfRefills, listOfService, listOfProfit;
        private TextBlock totalExpesns_exp, dayExpesns_exp, totalMileage_exp, dayMilage_exp, TitleExpOrProf;
        private ApplicationContext db;
        private Car userCar;
        private User user;

        public SelectDateRadius(Run dateRadius_exp,
            ListBox listOfExpenses,
            Run countNote_exp,
            TextBlock totalExpesns_exp,
            TextBlock dayExpesns_exp,
            TextBlock totalMileage_exp,
            TextBlock dayMilage_exp,
            ListBox listOfRefills,
            ListBox listOfService,
            ListBox listOfProfit,
            TextBlock TitleExpOrProf,
            Car userCar,
            User user)
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
            this.listOfProfit = listOfProfit;
            this.TitleExpOrProf = TitleExpOrProf;
            this.userCar = userCar;
            this.user = user;
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public List<Expense> AddDateRadiusEXpension(string expenseDateStart, string expenseDateFinish)
        {
            DataClass dataClass = new DataClass();

            var test = dataClass.selectQuery("select * from Expenses where Expense_date BETWEEN  '" + expenseDateStart +
                                             "' AND '" + expenseDateFinish + "'" + " AND Car_id = " + userCar.Car_id).Rows;

            var filtersDateList = dataClass.SelecExpenses(test);

            mainWindow.FilterExpense(ref listOfExpenses, new ObservableCollection<Expense>(), new List<Expense>(), filtersDateList, listOfRefills, listOfService);

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

            return tempList;
        }

        public List<Profit> AddDateRadiusProfit(string expenseDateStart, string expenseDateFinish, ref ListBox listOfProfit)
        {
            db = new ApplicationContext();
            List<Profit> result = new List<Profit>();
            List<Profit> userProfit = db.Profits.Where(pr => pr.User_id == user.User_id).ToList();

            foreach (var profit in userProfit)
            {
                DateTime timeProf = DateTime.Parse(profit.Profit_date);
                if (timeProf >= DateTime.Parse(expenseDateStart) && timeProf <= DateTime.Parse(expenseDateFinish))
                {
                    result.Add(profit);
                }
            }

            listOfProfit.ItemsSource = new ObservableCollection<Profit>(result);
            return result;
        }
        private void AddDateRadius(object sender, RoutedEventArgs e)
        {
            DataClass dataClass = new DataClass();
            mainWindow = new MainWindow();

            bool isCorect = true;
            string expenseDateStart = startDate.Text;
            string expenseDateFinish = finishtDate.Text;

            mainWindow.ValidationDate(startDate, ref expenseDateStart, ref isCorect);
            mainWindow.ValidationDate(finishtDate, ref expenseDateFinish, ref isCorect);

            Dictionary<string, double> calcutaeStatistick;

            if (listOfProfit != null)
            {
                var profitList = AddDateRadiusProfit( expenseDateStart, expenseDateFinish,ref listOfProfit);
                calcutaeStatistick = CountTotalCostProfit(profitList);
                countNote_exp.Text = profitList.Count() + "";
                TitleExpOrProf.Text = "Прибыль за период";
            }
            else
            {
                var expenseList = AddDateRadiusEXpension(expenseDateStart, expenseDateFinish);
                calcutaeStatistick = CountTotalCost(expenseList);
                countNote_exp.Text = expenseList.Count() + "";
                TitleExpOrProf.Text = "Траты за период";
            }

            dateRadius_exp.Text = "(" + expenseDateStart.Replace("-", "/") + " - " +
                                  expenseDateFinish.Replace("-", "/") + ")"; // show date raius
            totalExpesns_exp.Text = calcutaeStatistick["totalCost"] + " ₴";
            dayExpesns_exp.Text = calcutaeStatistick["averageCost"] + " ₴";
            totalMileage_exp.Text = calcutaeStatistick["resultMileage"] + " км";
            dayMilage_exp.Text = calcutaeStatistick["averageMileage"]+ " км";

            dateRadius_exp.Text = "(" + expenseDateStart.Replace("-", "/") + " - " +
                                  expenseDateFinish.Replace("-", "/") + ")"; // show date raius
            totalExpesns_exp.Text = calcutaeStatistick["totalCost"] + " ₴";
            dayExpesns_exp.Text = calcutaeStatistick["averageCost"] + " ₴";
            totalMileage_exp.Text = calcutaeStatistick["resultMileage"] + " км";
            dayMilage_exp.Text = calcutaeStatistick["averageMileage"] + " км";
            Close();
            
        }

        public Dictionary<string, double> CountTotalCost(List<Expense> filtersDateList)//get all values for general statistic
        {
            Dictionary<string, double> resultDictionary = new Dictionary<string, double>();
            double result = 0;
            foreach (Expense itemExpense in filtersDateList)
            {
                result += itemExpense.TotalCost;
            }

            double startMileage = filtersDateList[0].Mileage_num;
            double finMileage = filtersDateList[filtersDateList.Count-1].Mileage_num;
            double resultMileage = finMileage - startMileage;

            resultDictionary.Add("totalCost",Math.Round(result,2) );
            resultDictionary.Add("averageCost",Math.Round(result/ filtersDateList.Count,2));
            resultDictionary.Add("resultMileage",Math.Round(resultMileage,2));
            resultDictionary.Add("averageMileage",Math.Round(resultMileage / filtersDateList.Count));


            return resultDictionary;
        }

        public Dictionary<string, double> CountTotalCostProfit(List<Profit> filtersDateList)//get all values for general statistic
        {
            Dictionary<string, double> resultDictionary = new Dictionary<string, double>();
            double result = 0;
            foreach (Profit itemProfit in filtersDateList)
            {
                result += itemProfit.ProfitMargin_num;
            }

            double startMileage = filtersDateList[0].Mileage_num;
            double finMileage = filtersDateList[filtersDateList.Count - 1].Mileage_num;
            double resultMileage = finMileage - startMileage;

            resultDictionary.Add("totalCost", Math.Round(result, 2));
            resultDictionary.Add("averageCost", Math.Round(result / filtersDateList.Count, 2));
            resultDictionary.Add("resultMileage", Math.Round(resultMileage, 2));
            resultDictionary.Add("averageMileage", Math.Round(resultMileage / filtersDateList.Count));


            return resultDictionary;
        }

    }
}
