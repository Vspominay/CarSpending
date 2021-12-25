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
using System.Windows.Forms;
using System.Data.SQLite;
using MessageBox = System.Windows.MessageBox;

namespace CarSpending.prompt
{
    /// <summary>
    /// Логика взаимодействия для UserQuestions.xaml
    /// </summary>
    public partial class UserQuestions : Window
    {

        public UserQuestions()
        {
            InitializeComponent();
        }

        private void AddData(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SQLiteConnection sqlite = new SQLiteConnection("Data Source=.\\spendingCar.db"))
                {
                    DataSet dataSet = new DataSet();
                    SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(Input_Box.Text, sqlite);
                    dataAdapter.Fill(dataSet);

                    Output.ItemsSource = dataSet.Tables[0].DefaultView;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Error", MessageBoxButton.OK);
            }
          
        }
    }
}