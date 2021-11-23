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
    /// Логика взаимодействия для DeleteReminder.xaml
    /// </summary>
    public partial class DeleteReminder : Window
    {
        private DataClass dataClass;
        private int reminderId;
        private User user;
        public DeleteReminder(int reminderId,User user)
        {
            InitializeComponent();
            this.reminderId = reminderId;
            dataClass = new DataClass();
            this.user = user;
        }

        private void CancelDelete_click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DeleteSeltReminder(object sender, RoutedEventArgs e)
        {
            dataClass.selectQuery("delete from Reminders where Reminder_id = " + reminderId);
            MainWindow mainWindow = new MainWindow(user);
            mainWindow.Show();
            Close();
        }
    }
}
