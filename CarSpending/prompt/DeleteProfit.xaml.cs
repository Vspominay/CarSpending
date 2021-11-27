using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для DeleteProfit.xaml
    /// </summary>
    public partial class DeleteProfit : Window
    {
        private DataClass dataClass;
        private Profit selectProfit;
        public DeleteProfit(DataClass dataClass,Profit selectProfit)
        {
            this.dataClass = dataClass;
            this.selectProfit = selectProfit;
            InitializeComponent();
        }
        private void DeleteSelectedExpense(object sender, RoutedEventArgs e)
        {
            try
            {
                dataClass.selectQuery("DELETE FROM Profits Where Profit_id = " + selectProfit.Profit_id);

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception + "", "Error", MessageBoxButton.OK);
            }
            Close();
        }

        private void CancelDeleteExpense_click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
