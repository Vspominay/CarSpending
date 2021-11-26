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
        private ListBox listOfExpenses, listRef,listSer;
        private DataClass dataClass;
        private MainWindow mainWindow;
        public SortWindow(ListBox listOfExpenses,ListBox listOfRefills,ListBox listOfService)
        {
            InitializeComponent();
            this.listOfExpenses = listOfExpenses;
            listRef = listOfRefills;
            listSer = listOfService;

        }

        private void sortedItems_click(object sender, RoutedEventArgs e)
        {
            
            SortedItems(ref listOfExpenses, listRef, listSer);
        }

        private void closeWindow_click(object sender, RoutedEventArgs e)
        {
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
                if (DescRad.IsChecked == true)
                {
                    itemCost = dataClass.selectQuery("SELECT * from Expenses ORDER by TotalCost DESC").Rows;
                }
                else
                {
                    itemCost = dataClass.selectQuery("SELECT * from Expenses ORDER by TotalCost").Rows;
                }
            }
            else if(sortedMileage.IsChecked == true)
            {
                if (DescRad.IsChecked == true)
                {
                    itemCost = dataClass.selectQuery("SELECT * from Expenses ORDER by Mileage_num DESC").Rows;
                }
                else
                {
                    itemCost = dataClass.selectQuery("SELECT * from Expenses ORDER by Mileage_num").Rows;
                }
            }
            else if (sortedTime.IsChecked == true)
            {
                if (DescRad.IsChecked == true)
                {
                    itemCost = dataClass.selectQuery("SELECT * from Expenses ORDER by Expense_date DESC").Rows;
                }
                else
                {
                    itemCost = dataClass.selectQuery("SELECT * from Expenses ORDER by Expense_date").Rows;
                }
            }
            var filtersDateList = dataClass.SelecExpenses(itemCost);
            mainWindow.FilterExpense(ref listOfExpenses,new ObservableCollection<Expense>(),new List<Expense>(), filtersDateList,listRef,listSer);
            //listOfExpenses.ItemsSource = new ObservableCollection<Expense>(filtersDateList);// push items into listblock with expenses
            Close();
        }
    }
}
