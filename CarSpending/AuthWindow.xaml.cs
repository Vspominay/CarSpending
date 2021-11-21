using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;

namespace CarSpending
{
    /// <summary>
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        private ApplicationContext db;

        public AuthWindow()
        {
            InitializeComponent();
            db = new ApplicationContext();
        }

        private void ButtonAuth_click(object sender, RoutedEventArgs e) //click on the button for the switch between blocks
        {
            RegBlock.Visibility = Visibility.Hidden;
            AuthBlock.Visibility = Visibility.Visible;
            СheckAuth.IsChecked = true;
        }

        private void ButtonReg_click(object sender, RoutedEventArgs e) //click on the button for the switch between blocks
        {
            AuthBlock.Visibility = Visibility.Hidden;
            RegBlock.Visibility = Visibility.Visible;
            СheckReg.IsChecked = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e) //click on the button for registration
        {
            string userName = UserName_reg.Text.Trim();
            string userLogin = UserLogin_reg.Text.Trim();
            string userEmail = UserEmail_reg.Text.Trim().ToLower();
            string userPassword = UserPass_reg.Password.Trim();
            string userPasswordAgain = UserPassAgain_reg.Password.Trim();
            string patternEmail =
                @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";
            string patternText = @"[-!#\$%&'\*\+/=\?\@]";

            bool isCorect = true;

            if (userName.Length < 3 || Regex.IsMatch(userName, patternText, RegexOptions.IgnoreCase))
            {
                UserName_reg.Foreground = Brushes.DarkRed;
                TextFieldAssist.SetUnderlineBrush(UserName_reg, Brushes.DarkRed);
                HintAssist.SetHint(UserName_reg, "Некоректно заполненое поле ");
                isCorect = false;
            }

            if (userLogin.Length < 3 || Regex.IsMatch(userLogin, patternText, RegexOptions.IgnoreCase))
            {
                UserLogin_reg.Foreground = Brushes.DarkRed;
                TextFieldAssist.SetUnderlineBrush(UserLogin_reg, Brushes.DarkRed);
                HintAssist.SetHint(UserLogin_reg, "Некоректно заполненое поле ");
                isCorect = false;

            }

            if (userEmail.Length < 3 || !Regex.IsMatch(userEmail, patternEmail, RegexOptions.IgnoreCase))
            {
                UserEmail_reg.Foreground = Brushes.DarkRed;
                TextFieldAssist.SetUnderlineBrush(UserEmail_reg, Brushes.DarkRed);
                HintAssist.SetHint(UserEmail_reg, "Некоректно заполненое поле ");
                isCorect = false;

            }

            if (userPassword.Length < 3)
            {
                UserPass_reg.Foreground = Brushes.DarkRed;
                TextFieldAssist.SetUnderlineBrush(UserPass_reg, Brushes.DarkRed);
                HintAssist.SetHint(UserPass_reg, "Пароль должен быть длинее 3 символов");
                isCorect = false;

            }

            if (userPasswordAgain != userPassword)
            {
                UserPassAgain_reg.Foreground = Brushes.DarkRed;
                TextFieldAssist.SetUnderlineBrush(UserPassAgain_reg, Brushes.DarkRed);
                HintAssist.SetHint(UserPassAgain_reg, "Пароли не совпадают");
                isCorect = false;

            }

            if (isCorect)
            {
                User newUser = new User(userName, userLogin, userPassword, userEmail);

                db.Users.Add(newUser);
                db.SaveChanges();

                RegBlock.Visibility = Visibility.Hidden;
                AuthBlock.Visibility = Visibility.Visible;
                СheckAuth.IsChecked = true;
            }
            
        }

        private void ButtonGoAuth_click(object sender, RoutedEventArgs e) //click on the button for login
        {
            
            bool isCorect = true;
            string userLogin = UserLogin_auth.Text.Trim();
            string userPassword = UserPassword_auth.Password.Trim();

            if (userLogin.Length == 0)
            {
                UserLogin_auth.Foreground = Brushes.DarkRed;
                TextFieldAssist.SetUnderlineBrush(UserLogin_auth, Brushes.DarkRed);
                HintAssist.SetHint(UserLogin_auth, "Логин. Поле не может быть пустым");
                isCorect = false;
            }

            if (userPassword.Length == 0)
            {
                UserPassword_auth.Foreground = Brushes.DarkRed;
                TextFieldAssist.SetUnderlineBrush(UserPassword_auth, Brushes.DarkRed);
                HintAssist.SetHint(UserPassword_auth, "Пароль. Поле не может быть пустым");
                isCorect = false;
            }

            if (isCorect)
            {
               

                User authUser;

                using (ApplicationContext db = new ApplicationContext())
                {
                    authUser = db.Users.FirstOrDefault(user =>
                        user.Login == userLogin && user.Password == userPassword);
                }

                if (authUser != null)
                {
                    MainWindow mainWindow = new MainWindow(authUser);
                    mainWindow.Show();
                    Close();
                }
                else
                {
                    promptNotFoundUser promptNotFound = new promptNotFoundUser();
                    promptNotFound.Owner = this;
                    promptNotFound.ShowDialog();
                }
            }

        }
    }
}
