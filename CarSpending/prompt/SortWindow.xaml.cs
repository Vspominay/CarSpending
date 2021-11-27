using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
    /// Логика взаимодействия для SortWindow.xaml
    /// </summary>
    public partial class SortWindow : Window
    {
        private ListBox listOfExpenses, listRef,listSer, listOfProfit;
        private DataClass dataClass;
        private MainWindow mainWindow;
        private ApplicationContext db;
        public SortWindow(ListBox listOfExpenses,ListBox listOfRefills,ListBox listOfService,ListBox listOfProfit)
        {
            InitializeComponent();
            this.listOfExpenses = listOfExpenses;
            this.listOfProfit = listOfProfit;
            listRef = listOfRefills;
            listSer = listOfService;

        }

        private void sortedItems_click(object sender, RoutedEventArgs e)
        {
            if (listOfProfit == null)
            {
                SortedItems(ref listOfExpenses, listRef, listSer);
            }
            else
            {
                SortedProfits(ref listOfProfit);
            }
        }

        private void closeWindow_click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void SortedProfits(ref ListBox listOfProfits)
        {
            db = new ApplicationContext();
            var sortedCost = sortedCost_rad;
            var sortedMileage = sortedMileage_rad;
            var sortedTime = sortedTime_rad;
            var DescRad = sortedDesc;


            dataClass = new DataClass();
            mainWindow = new MainWindow();
            List<Profit> listProf = new List<Profit>();

            if (sortedCost.IsChecked == true)
            {
                listProf = DescRad.IsChecked == true ? db.Profits.OrderByDescending(p=>p.ProfitMargin_num).ToList() : db.Profits.OrderBy(p => p.ProfitMargin_num).ToList();
            }
            else if (sortedMileage.IsChecked == true)
            {
                listProf = DescRad.IsChecked == true ? db.Profits.OrderByDescending(p => p.Mileage_num).ToList() : db.Profits.OrderBy(p => p.Mileage_num).ToList();
            }
            else if (sortedTime.IsChecked == true)
            {
                listProf = DescRad.IsChecked == true ? db.Profits.OrderByDescending(p => p.Profit_date).ToList() : db.Profits.OrderBy(p => p.Profit_date).ToList();
            }

            listOfProfits.ItemsSource = new ObservableCollection<Profit>(listProf);
            Close();
        }

        public void SortedItems(ref ListBox listOfExpenses,ListBox listRef,ListBox listSer)
        {
            var sortedCost = sortedCost_rad;
            var sortedMileage = sortedMileage_rad;
            var sortedTime = sortedTime_rad;
            var DescRad = sortedDesc;

            DataRowCollection itemCost = null;
            dataClass = new DataClass();
            mainWindow = new MainWindow();

            if (sortedCost.IsChecked == true )
            {
                itemCost = DescRad.IsChecked == true ? dataClass.selectQuery("SELECT * from Expenses ORDER by TotalCost DESC").Rows : dataClass.selectQuery("SELECT * from Expenses ORDER by TotalCost").Rows;
            }
            else if(sortedMileage.IsChecked == true)
            {
                itemCost = DescRad.IsChecked == true ? dataClass.selectQuery("SELECT * from Expenses ORDER by Mileage_num DESC").Rows : dataClass.selectQuery("SELECT * from Expenses ORDER by Mileage_num").Rows;
            }
            else if (sortedTime.IsChecked == true)
            {
                itemCost = DescRad.IsChecked == true ? dataClass.selectQuery("SELECT * from Expenses ORDER by Expense_date DESC").Rows : dataClass.selectQuery("SELECT * from Expenses ORDER by Expense_date").Rows;
            }
            var filtersDateList = dataClass.SelecExpenses(itemCost);
            mainWindow.FilterExpense(ref listOfExpenses,new ObservableCollection<Expense>(),new List<Expense>(), filtersDateList,listRef,listSer);
            Close();
        }
    }
}
