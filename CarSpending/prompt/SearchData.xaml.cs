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


namespace CarSpending.prompt
{
    /// <summary>
    /// Логика взаимодействия для SearchData.xaml
    /// </summary>
    public partial class SearchData : Window
    {
        private ListBox listOfExpenses, listOfProfits;
        private MainWindow main;
        private DataClass dataClass;
        private ApplicationContext db;
        private Car userCar;
        private User user;
        public SearchData(ListBox listOfExpenses,ListBox listOfProfits,Car userCar, User user)
        {
            db = new ApplicationContext();
            this.listOfExpenses = listOfExpenses;
            this.listOfProfits = listOfProfits;
            this.userCar = userCar;
            this.user = user;
            InitializeComponent();
        }

        private void closeWindow_search(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void searchData_click(object sender, RoutedEventArgs e)
        {
            main = new MainWindow();
            dataClass = new DataClass();



            var serachCost = searchCost_rad;
            var searchMileage = searchMileage_rad;
            var inputSearch = searchInput;
            double totalCost = 0;
            double mileage = 0;
            bool isCorect = true;
            string patternText = @"[-!#\$%&'\*\+/=\?\@]";


            if (serachCost.IsChecked == true)
            {
                main.validationCost(inputSearch, ref totalCost, ref isCorect, patternText);
                if (isCorect)
                {
                    if (listOfExpenses == listOfProfits)
                    {
                        var filtersDateListProf = db.Profits.Where(p => p.ProfitMargin_num == totalCost && p.User_id == user.User_id);
                        listOfExpenses.ItemsSource = new ObservableCollection<Profit>(filtersDateListProf);
                    }
                    else
                    {
                        var itemCost = dataClass.selectQuery("select * from Expenses where TotalCost = " + totalCost + " AND Car_id = " + userCar.Car_id)
                            .Rows;
                        var filtersDateList = dataClass.SelecExpenses(itemCost);
                        listOfExpenses.ItemsSource =
                            new ObservableCollection<Expense>(
                                filtersDateList); // push items into listblock with expenses
                    }

                    Close();
                }
            }
            else if (searchMileage.IsChecked == true)
            {
                main.validationCost(inputSearch, ref mileage, ref isCorect, patternText);
                if (isCorect)
                {
                    if (listOfExpenses == listOfProfits)
                    {
                        var filtersDateListProf = db.Profits.Where(p => p.Mileage_num == mileage && p.User_id == user.User_id);
                        listOfExpenses.ItemsSource = new ObservableCollection<Profit>(filtersDateListProf);
                    }
                    else
                    {
                        var itemCost = dataClass.selectQuery("select * from Expenses where Mileage_num = " + mileage + " AND Car_id = " + userCar.Car_id).Rows;
                        var filtersDateList = dataClass.SelecExpenses(itemCost);
                        listOfExpenses.ItemsSource = new ObservableCollection<Expense>(filtersDateList);// push items into listblock with expenses
                    }

                    Close();
                }
            }


        }
    }
}
