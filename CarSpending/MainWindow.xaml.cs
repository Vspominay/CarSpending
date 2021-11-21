using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CarSpending.prompt;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using CarSpending.ListboxItems;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using f = System.Windows.Forms;

namespace CarSpending
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private ApplicationContext db;
        public ObservableCollection<FuelType> FuelTypes { get; set; }

        public ObservableCollection<TypeOfserviceItem> TypeOfserviceItems { get; set; }
        public ObservableCollection<ProfitType> ProfitTypes { get; set; }
        public ObservableCollection<Reminder> RemindersList { get; set; }




        private User user;
        private List<Car> userCars;
        private List<Image> carImages;
        private List<FuelType> fuelTypes;
        private List<Refill> refills;
        private List<Expense> expenses;
        private List<Favor> favors;
        private List<Service> services;
        private List<string> serviceActiveItems = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(User user)
        {
            InitializeComponent();
            hiTitle.Text = "Здравствуйте, " + user.First_name + "!";
            this.user = user;
            db = new ApplicationContext();

            TypeOfserviceItems = new ObservableCollection<TypeOfserviceItem>();
            foreach (var typeItem in db.FavorTypes)
            {
                TypeOfserviceItems.Add(new TypeOfserviceItem(typeItem.Favor_name, 0,
                    1)); //push elements about type of service
            }

            serviceType_list.ItemsSource = TypeOfserviceItems;

            pushIntoListbox(FuelTypes, typesOfFuel, db.FuelTypes); //push type services into listbox
            pushIntoListbox(ProfitTypes, typeOfProfit, db.ProfitTypes); //push profit types into listbox
            pushIntoListbox(RemindersList, reminders_list, db.Reminders); //push reminders into listbox
        }

        public void pushIntoListbox<T>(ObservableCollection<T> observableCollection, ListBox listBox,DbSet dbSet)
        {
            observableCollection = new ObservableCollection<T>();
            foreach (T setObject in dbSet)
            {
                observableCollection.Add(setObject); //push elements about remiders
            }

            listBox.ItemsSource = observableCollection;
        }
        private void TabItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            promptBeforeExit promptExit = new promptBeforeExit();
            promptExit.Owner = this;
            promptExit.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e) // button for adding car
        {
            AddCar addcacCar = new AddCar(user);
            addcacCar.Owner = this;
            addcacCar.ShowDialog();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                userCars = db.Cars.Where(c => c.User_id == user.User_id).ToList(); // upload data about user cars
                if (userCars != null)
                {
                    foreach (var car in userCars)
                    {
                        ComboBoxCars.Items.Add(car.CarBrand);
                    }

                    ComboBoxCars.SelectedIndex = 0;
                }
            }
        }

        private void
            ComboBoxCars_OnSelected(object sender, RoutedEventArgs e) // events that occur when switching combo boxes
        {
            carBrand_c.Text = userCars[ComboBoxCars.SelectedIndex].CarBrand;
            VIN_c.Text = userCars[ComboBoxCars.SelectedIndex].Vin_num;
            mileage_c.Text = userCars[ComboBoxCars.SelectedIndex].Mileage_num + " км";
            tankVolume_c.Text = userCars[ComboBoxCars.SelectedIndex].TankVolume_num + " л";
            fuelTitleText.Text = "Добавление новой записи про зарпавку для автомобиля " +
                                 userCars[ComboBoxCars.SelectedIndex].CarBrand;
            serviceTitleText.Text = "Добавление новой записи про сервис для автомобиля " +
                                    userCars[ComboBoxCars.SelectedIndex].CarBrand;
            profitTitleText.Text = "Добавление новой записи про доход для автомобиля " +
                                   userCars[ComboBoxCars.SelectedIndex].CarBrand;
            reminderTitleText.Text = "Добавление нового напоминания для пользователя " + user.First_name;
            statisticTitleText.Text =
                "просмотр статистики для автомобиля " + userCars[ComboBoxCars.SelectedIndex].CarBrand;

            if (ComboBoxCars.SelectedIndex != -1 && userCars[ComboBoxCars.SelectedIndex].Image_id != 0)
            {
                carImages = db.Images.ToList()
                    .Where(i => i.Image_id == userCars[ComboBoxCars.SelectedIndex].Image_id).ToList();
                GetByteImg(carImages[0].Image_id, carImages[0].ImagePath);
            }
            else
            {
                SelectImg.ImageSource = new BitmapImage(new Uri("../../img/CarImg.png", UriKind.Relative))
                    { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
            }
        }

        private void Image_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) // click on the imagecar
        {
            var o = new f.OpenFileDialog();
            o.Filter = "Выберите картинку | *.png;  *.jpg;";
            if (o.ShowDialog() == f.DialogResult.OK)
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(o.FileName, UriKind.Relative);
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.DecodePixelWidth = 320;
                bitmapImage.EndInit();

                GetImgFolder(bitmapImage);
            }
        }


        void GetByteImg(int ID, string Byteg)
        {
            byte[] imgB = Byteg.Split(';').Select(a => byte.Parse(a)).ToArray();
            MemoryStream ms = new MemoryStream(imgB);
            SelectImg.ImageSource = BitmapFrame.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
        }

        void GetImgFolder(BitmapImage bitmapImage) // image encoding into bytes
        {
            PngBitmapEncoder pe = new PngBitmapEncoder();
            MemoryStream ms = new MemoryStream();
            StringBuilder sb = new StringBuilder();
            pe.Frames.Add(BitmapFrame.Create(bitmapImage));
            pe.Save(ms);
            byte[] imgB = ms.ToArray();
            foreach (var b in imgB)
                sb.Append(b).Append(';');
            sb.Remove(sb.Length - 1, 1);

            GetImgDB(sb.ToString());
        }


        void GetImgDB(string Get) // add img to the database
        {
            Image newImage = new Image(Get);

            db.Images.Add(newImage);

            db.SaveChanges();

            using (ApplicationContext context = new ApplicationContext()) // giving a car needed image id
            {
                var car = context.Cars.ToList()
                    .FirstOrDefault(c => c.Car_id == userCars[ComboBoxCars.SelectedIndex].Car_id);
                var addedImage = context.Images.ToList().FirstOrDefault(i => i.ImagePath == newImage.ImagePath);

                car.Image_id = addedImage.Image_id;

                context.SaveChanges();
            }

            MainWindow main = new MainWindow(user);
            main.Show();
            Close();
        }

        private void typesOfFuel_SelectionChanged(object sender, SelectionChangedEventArgs e) // fill input about type of fuel
        {
            var fuelSelect = (FuelType)typesOfFuel.SelectedItem;
            inputTypeOfFuel.Text = fuelSelect.Fuel_title;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e) // active radio buttons
        {
            var cb = sender as RadioButton;
            var item = cb.DataContext;
            typesOfFuel.SelectedItem = item;
        }

        public void ValidationMileage(TextBox mileage,ref double carMileage,ref bool isCorect)// validation car mileage
        {
            if (mileage.Text.Length <= 0 ||
                !Regex.IsMatch(mileage.Text, @"^[0-9,.]*$", RegexOptions.IgnoreCase)) // check car mileage
            {
                mileage.Foreground = Brushes.DarkRed;
                TextFieldAssist.SetUnderlineBrush(mileage, Brushes.DarkRed);
                HintAssist.SetHint(mileage, "Некоректно заполненое поле ");
                isCorect = false;
            }
            else
            {
                carMileage = Convert.ToDouble(mileage.Text.Trim().Contains('.')
                    ? mileage.Text.Trim().Replace('.', ',')
                    : mileage.Text.Trim());
                
            }
        }

        public void validationCost(TextBox costBox,ref double cost,ref bool isCorect, string patternText)
        {
            if (costBox.Text.Length <= 0 || Regex.IsMatch(costBox.Text, patternText,
                                             RegexOptions.IgnoreCase)
                                         || Regex.IsMatch(costBox.Text, @"[a-zA-Z]",
                                             RegexOptions.IgnoreCase)) // check liter cost
            {
                costBox.Foreground = Brushes.DarkRed;
                TextFieldAssist.SetUnderlineBrush(costBox, Brushes.DarkRed);
                HintAssist.SetHint(costBox, "Некоректно заполненое поле");
                isCorect = false;
            }
            else
            {
                cost = Convert.ToDouble(costBox.Text.Trim().Contains('.') ? costBox.Text.Trim().Replace('.', ',') : costBox.Text.Trim());
            }
        }

        public void ValidationDate(DatePicker datePicker,ref string ExpenseDate, ref bool isCorect)
        {
            DateTime dt;
            if (DateTime.TryParse(datePicker.Text, out dt))
            {
                ExpenseDate = ToDateSqlite(DateTime.Parse(datePicker.Text));
            }
            else
            {
                isCorect = false;
                datePicker.Foreground = Brushes.DarkRed;
                TextFieldAssist.SetUnderlineBrush(datePicker, Brushes.DarkRed);
                HintAssist.SetHint(datePicker, "Некоректно заполненое поле");
            }
        }
        private void addNoteAboutRefill_click(object sender, RoutedEventArgs e) // add note about refill
        {
            var fuelSelect = (FuelType)typesOfFuel.SelectedItem;
            double carMileage = 0;
            int fuelId = -1;
            double literCost = 0;
            double amountLiter = 0;
            int fullTank = 0;
            bool isCorect = true;
            string expenseDate = "";

            string patternText = @"[-!#\$%&'\*\+/=\?\@]";

            ValidationMileage(mileage_refill,ref carMileage,ref isCorect);

            /*if (mileage_refill.Text.Length <= 0 ||
                !Regex.IsMatch(mileage_refill.Text, @"^[0-9,.]*$", RegexOptions.IgnoreCase)) // check car mileage
            {
                mileage_refill.Foreground = Brushes.DarkRed;
                TextFieldAssist.SetUnderlineBrush(mileage_refill, Brushes.DarkRed);
                HintAssist.SetHint(mileage_refill, "Некоректно заполненое поле ");
                isCorect = false;
            }
            else
            {
                if (mileage_refill.Text.Trim().Contains('.'))
                {
                    CarMileage = Convert.ToDouble(mileage_refill.Text.Trim().Replace('.', ','));
                }
                else
                {
                    CarMileage = Convert.ToDouble(mileage_refill.Text.Trim());
                }
            }*/



            validationCost(literCost_refill,ref literCost,ref isCorect, patternText);
            /*if (literCost_refill.Text.Length <= 0 || Regex.IsMatch(literCost_refill.Text, patternText,
                                                      RegexOptions.IgnoreCase)
                                                  || Regex.IsMatch(literCost_refill.Text, @"[a-zA-Z]",
                                                      RegexOptions.IgnoreCase)) // check liter cost
            {
                literCost_refill.Foreground = Brushes.DarkRed;
                TextFieldAssist.SetUnderlineBrush(literCost_refill, Brushes.DarkRed);
                HintAssist.SetHint(literCost_refill, "Некоректно заполненое поле");
                isCorect = false;
            }
            else
            {
                if (literCost_refill.Text.Trim().Contains('.'))
                {
                    LiterCost = Convert.ToDouble(literCost_refill.Text.Trim().Replace('.', ','));
                }
                else
                {
                    LiterCost = Convert.ToDouble(literCost_refill.Text.Trim());
                }
            }*/

            validationCost(amountLiters_refill,ref amountLiter,ref isCorect, patternText);
            /*if (amountLiters_refill.Text.Length <= 0 ||
                Regex.IsMatch(amountLiters_refill.Text, patternText, RegexOptions.IgnoreCase)) // check Amount Liter
            {
                amountLiters_refill.Foreground = Brushes.DarkRed;
                TextFieldAssist.SetUnderlineBrush(amountLiters_refill, Brushes.DarkRed);
                HintAssist.SetHint(amountLiters_refill, "Некоректно заполненое поле");
                isCorect = false;
            }
            else
            {
                if (amountLiters_refill.Text.Trim().Contains('.'))
                {
                    AmountLiter = Convert.ToDouble(amountLiters_refill.Text.Trim().Replace('.', ','));
                }
                else
                {
                    AmountLiter = Convert.ToDouble(amountLiters_refill.Text.Trim());
                }
            }*/

            if (Fulltank_refill.IsChecked == true) fullTank = 1;//checking full tank or no


            if (inputTypeOfFuel.Text.Length < 1) //checking correct data entry in the type of fuel field
            {
                inputTypeOfFuel.Foreground = Brushes.DarkRed;
                TextFieldAssist.SetUnderlineBrush(inputTypeOfFuel, Brushes.DarkRed);
                HintAssist.SetHint(inputTypeOfFuel, "Выберите тип топлива в блоке справа!");
                isCorect = false;
            }
            else
            {
                fuelId = db.FuelTypes.ToList().FirstOrDefault(fuel => fuel.Fuel_title == fuelSelect.Fuel_title).Fuel_id;
            }

            ValidationDate(expenseDate_refill, ref expenseDate, ref isCorect);

            // if (DateTime.TryParse(expenseDate_refill.Text, out dt))
            // {
            //     ExpenseDate = ToDateSqlite(DateTime.Parse(expenseDate_refill.Text));
            // }
            // else
            // {
            //     isCorect = false;
            //     expenseDate_refill.Foreground = Brushes.DarkRed;
            //     TextFieldAssist.SetUnderlineBrush(expenseDate_refill, Brushes.DarkRed);
            //     HintAssist.SetHint(expenseDate_refill, "Некоректно заполненое поле");
            // }

            if (isCorect)
            {
                Refill newRefill = new Refill(fuelId, literCost, amountLiter, fullTank);

                db.Refills.Add(newRefill);
                db.SaveChanges();

                using (ApplicationContext context = new ApplicationContext())
                {
                    var totalCost = amountLiter * literCost;

                    context.Expenses.Add(new Expense(carId: userCars[ComboBoxCars.SelectedIndex].Car_id,
                        expanseDate: expenseDate,
                        mileage: carMileage, totalCost: totalCost, comment: comment_refill.Text,
                        serviceId: 0,
                        refillId: newRefill.Refill_id, location: location_refill.Text));
                    context.SaveChanges();
                }

                SnackbarTwo.MessageQueue?.Enqueue("Запись добавлена!", null, null, null, false, true, TimeSpan.FromSeconds(3));
                expenseDate_refill.Text = "";
                mileage_refill.Text = "";
                literCost_refill.Text = "";
                comment_refill.Text = "";
                Fulltank_refill.IsChecked = false;
                amountLiters_refill.Text = "";
                location_refill.Text = "";
            }
        }

        public static string ToDateSqlite(DateTime dateTime) // convert date from input field to SqliteFormat
        {
            string formatDate = "yyyy-MM-dd";
            return dateTime.ToString(formatDate);
        }

        private void AddNewTypeService_Click(object sender, RoutedEventArgs e)
        {
            AddServiceType addService = new AddServiceType(user);
            addService.Owner = this;
            addService.ShowDialog();
        }

        private void
            ServiceType_list_OnSelectionChanged(object sender,
                SelectionChangedEventArgs e) // change service type select item
        {
            var serviceSelect = (TypeOfserviceItem)serviceType_list.SelectedItem;

            if (serviceActiveItems.Count != 0 &&
                serviceActiveItems.Contains(((TypeOfserviceItem)serviceType_list.SelectedItem).Favor_name))
            {
                serviceActiveItems.Remove(((TypeOfserviceItem)serviceType_list.SelectedItem).Favor_name);
            }
            else
            {
                serviceActiveItems.Add(((TypeOfserviceItem)serviceType_list.SelectedItem).Favor_name);
            }

            updateInputService(serviceActiveItems, inputTypeOfServices);
        }

        private void activeCheckBoxService_click(object sender, RoutedEventArgs e)// click at the checkbox 
        {
            var cb = sender as CheckBox;
            var item = cb.DataContext;

            if (serviceType_list.SelectedItem == item && serviceActiveItems.Contains(((TypeOfserviceItem)serviceType_list.SelectedItem).Favor_name))
            {
                serviceActiveItems.Remove(((TypeOfserviceItem)serviceType_list.SelectedItem).Favor_name);
                updateInputService(serviceActiveItems, inputTypeOfServices);

            }
            else if (serviceType_list.SelectedItem == item && !serviceActiveItems.Contains(((TypeOfserviceItem)serviceType_list.SelectedItem).Favor_name))
            {
                serviceActiveItems.Add(((TypeOfserviceItem)serviceType_list.SelectedItem).Favor_name);
                updateInputService(serviceActiveItems, inputTypeOfServices);
            }
            serviceType_list.SelectedItem = item;

        }

        public void updateInputService(List<string> serviceActiveItems, TextBox inputTypeOfServices)// function for update input field about active service
        {
            inputTypeOfServices.Text = "";
            literCost_service.Text = "";

            double totalCost = 0;
            foreach (var itemServ in serviceActiveItems)
            {
                inputTypeOfServices.Text += itemServ + " / ";
            }
        }

        private void addNoteAboutService_click(object sender, RoutedEventArgs e)// add note about service into database
        {
            addNoteIntoFavor(serviceType_list, serviceActiveItems);
            addNoteAboutService();
        }

        private void addNoteIntoFavor(ListBox serviceType_list, List<string> serviceActiveItems)
        {
            int favorCost = 0;
            int favorNum = 0;

            favors = new List<Favor>();
            foreach (var fav in serviceType_list.Items)
            {
                var newFav = (TypeOfserviceItem)fav;
                if (serviceActiveItems.Contains(newFav.Favor_name))
                {
                    var favorTypeId = db.FavorTypes.ToList().FirstOrDefault(type => type.Favor_name == newFav.Favor_name)
                        .FavorType_id;
                    favors.Add(new Favor(favorTypeId, newFav.FavorCost_num));
                }
            }

            db.Favors.AddRange(favors);
            db.SaveChanges();
        }

        private void addNoteAboutService()
        {
            double carMileage = 0;
            string expenseDate = "";
            double totalCost = 0;
            string comment = comment_service.Text;
            string location = location_service.Text;
            bool isCorect = true;

            DateTime dt;
            ValidationDate(expenseDate_service, ref expenseDate, ref isCorect);
            // if (DateTime.TryParse(expenseDate_service.Text, out dt))
            // {
            //     ExpenseDate = ToDateSqlite(DateTime.Parse(expenseDate_service.Text));
            // }
            // else
            // {
            //     isCorect = false;
            //     expenseDate_service.Foreground = Brushes.DarkRed;
            //     TextFieldAssist.SetUnderlineBrush(expenseDate_service, Brushes.DarkRed);
            //     HintAssist.SetHint(expenseDate_service, "Некоректно заполненое поле");
            // }

            ValidationMileage(mileage_service, ref carMileage, ref isCorect);
            /*if (mileage_service.Text.Length <= 0 ||
                !Regex.IsMatch(mileage_service.Text, @"^[0-9,.]*$", RegexOptions.IgnoreCase)) // check car mileage
            {
                mileage_service.Foreground = Brushes.DarkRed;
                TextFieldAssist.SetUnderlineBrush(mileage_service, Brushes.DarkRed);
                HintAssist.SetHint(mileage_service, "Некоректно заполненое поле ");
                isCorect = false;
            }
            else
            {
                if (mileage_service.Text.Trim().Contains('.'))
                {
                    CarMileage = Convert.ToDouble(mileage_service.Text.Trim().Replace('.', ','));
                }
                else
                {
                    CarMileage = Convert.ToDouble(mileage_service.Text.Trim());
                }
            }*/

            if (literCost_service.Text.Length == 0)
            {
                isCorect = false;
                literCost_service.Foreground = Brushes.DarkRed;
                TextFieldAssist.SetUnderlineBrush(literCost_service, Brushes.DarkRed);
                HintAssist.SetHint(literCost_service, "Некоректно заполненое поле ");
            }
            else
            {
                totalCost = Convert.ToInt32(literCost_service.Text);
            }

            if (isCorect)
            {
                Expense newExpense = new Expense
                (
                    carId: userCars[ComboBoxCars.SelectedIndex].Car_id,
                    expanseDate: expenseDate,
                    mileage: carMileage,
                    totalCost: totalCost,
                    comment: comment_service.Text,
                    refillId: 0,
                    serviceId: -1,
                    location: location_service.Text
                );

                db.Expenses.Add(newExpense);
                db.SaveChanges();

                var dataClass = new DataClass();

                using (ApplicationContext context = new ApplicationContext())
                {
                    services = new List<Service>();


                    foreach (var favor in favors)
                    {
                        string findFavorNameFromId = context.FavorTypes.ToList()
                            .FirstOrDefault(type => type.FavorType_id == favor.FavorType_id).Favor_name;
                        var favorNum =
                            TypeOfserviceItems.FirstOrDefault(item => item.Favor_name == findFavorNameFromId).Favor_num;

                        dataClass.selectQuery("INSERT into Services VALUES(" + newExpense.Expenses_id + "," +
                                              favor.Favor_id + "," + favorNum + ")");
                    }
                }

                SnackbarService.MessageQueue?.Enqueue("Запись добавлена!", null, null, null, false, true, TimeSpan.FromSeconds(3));
                expenseDate_service.Text = "";
                mileage_service.Text = "";
                literCost_service.Text = "";
                comment_service.Text = "";
                location_service.Text = "";
            }
        }


        private void LostFocuse(object sender, RoutedEventArgs e)// update data in total cost input
        {
            double totalCost = 0;
            foreach (TypeOfserviceItem type in serviceType_list.Items)
            {
                if (serviceActiveItems.Contains(type.Favor_name))
                {
                    totalCost += type.FavorCost_num * type.Favor_num;
                }
            }

            literCost_service.Text = totalCost + "";
        }

        private void addNewTypeProfit_click(object sender, RoutedEventArgs e)// add new type of profit into database
        {
            AddProfitType addProfit = new AddProfitType(user);
            addProfit.Owner = this;
            addProfit.ShowDialog();
        }

        private void TypeOfProfit_OnSelectionChanged(object sender, SelectionChangedEventArgs e)// fill input about type of prfit
        {
            var profitSelect = (ProfitType)typeOfProfit.SelectedItem;
            inputTypeOfProfit.Text = profitSelect.ProfitType_title;
        }

        private void ButtonBaseProfit_OnClick(object sender, RoutedEventArgs e) // active radio buttons
        {
            var cb = sender as RadioButton;
            var item = cb.DataContext;
            typeOfProfit.SelectedItem = item;
        }


        private void AddNoteAboutProfit_click(object sender, RoutedEventArgs e)// add note about profit into database
        {
            var profitSelet = (ProfitType)typeOfProfit.SelectedItem;
            double carMileage = 0;
            double profitMargin = 0;
            bool isCorect = true;
            string expenseDate = "";
            int profitId = -1;

            string patternText = @"[-!#\$%&'\*\+/=\?\@]";

            ValidationMileage(mileage_profit,ref carMileage,ref isCorect);
            /*if (mileage_profit.Text.Length <= 0 ||
                !Regex.IsMatch(mileage_profit.Text, @"^[0-9,.]*$", RegexOptions.IgnoreCase)) // check car mileage
            {
                mileage_profit.Foreground = Brushes.DarkRed;
                TextFieldAssist.SetUnderlineBrush(mileage_profit, Brushes.DarkRed);
                HintAssist.SetHint(mileage_profit, "Некоректно заполненое поле ");
                isCorect = false;
            }
            else
            {
                if (mileage_profit.Text.Trim().Contains('.'))
                {
                    CarMileage = Convert.ToDouble(mileage_profit.Text.Trim().Replace('.', ','));
                }
                else
                {
                    CarMileage = Convert.ToDouble(mileage_profit.Text.Trim());
                }
            }*/

            validationCost(literCost_profit,ref profitMargin,ref isCorect,patternText);
            /*if (literCost_profit.Text.Length <= 0 || Regex.IsMatch(literCost_profit.Text, patternText,
                                                      RegexOptions.IgnoreCase)
                                                  || Regex.IsMatch(literCost_profit.Text, @"[a-zA-Z]",
                                                      RegexOptions.IgnoreCase)) // check liter cost
            {
                literCost_profit.Foreground = Brushes.DarkRed;
                TextFieldAssist.SetUnderlineBrush(literCost_profit, Brushes.DarkRed);
                HintAssist.SetHint(literCost_profit, "Некоректно заполненое поле");
                isCorect = false;
            }
            else
            {
                if (literCost_profit.Text.Trim().Contains('.'))
                {
                    profitMargin = Convert.ToDouble(literCost_profit.Text.Trim().Replace('.', ','));
                }
                else
                {
                    profitMargin = Convert.ToDouble(literCost_profit.Text.Trim());
                }
            }*/

            
            if (inputTypeOfProfit.Text.Length < 1) //checking correct data entry in the type of profit field
            {
                inputTypeOfProfit.Foreground = Brushes.DarkRed;
                TextFieldAssist.SetUnderlineBrush(inputTypeOfProfit, Brushes.DarkRed);
                HintAssist.SetHint(inputTypeOfProfit, "Выберите тип топлива в блоке справа!");
                isCorect = false;
            }
            else
            {
                profitId = db.ProfitTypes.ToList().FirstOrDefault(prof => prof.ProfitType_title == profitSelet.ProfitType_title).ProfitType_id;
            }

            ValidationDate(expenseDate_profit,ref expenseDate,ref isCorect);
            // if (DateTime.TryParse(expenseDate_profit.Text, out dt))
            // {
            //     ExpenseDate = ToDateSqlite(DateTime.Parse(expenseDate_profit.Text));
            // }
            // else
            // {
            //     isCorect = false;
            //     expenseDate_profit.Foreground = Brushes.DarkRed;
            //     TextFieldAssist.SetUnderlineBrush(expenseDate_profit, Brushes.DarkRed);
            //     HintAssist.SetHint(expenseDate_profit, "Некоректно заполненое поле");
            // }

            if (isCorect)
            {

                Profit newProfit = new Profit(user.User_id, profitId, profitMargin,expenseDate,carMileage, comment_profit.Text);

                db.Profits.Add(newProfit);
                db.SaveChanges();

                SnackbarProfit.MessageQueue?.Enqueue("Запись добавлена!", null, null, null, false, true, TimeSpan.FromSeconds(3));
                expenseDate_profit.Text = "";
                mileage_profit.Text = "";
                literCost_profit.Text = "";
                comment_profit.Text = "";
            }
        }



        private void AddNewRemider_click(object sender, RoutedEventArgs e)// add new reminder
        {
            string ExpenseDate = "";
            bool isCorect = true;
            string titleReminder = tittle_remiders.Text;
            string patternText = @"[-!#\$%&'\*\+/=\?\@]";


            ValidationDate(expenseDate_reminders,ref ExpenseDate,ref isCorect);
            // if (DateTime.TryParse(expenseDate_reminders.Text, out dt))
            // {
            //     ExpenseDate = ToDateSqlite(DateTime.Parse(expenseDate_reminders.Text));
            // }
            // else
            // {
            //     isCorect = false;
            //     expenseDate_reminders.Foreground = Brushes.DarkRed;
            //     TextFieldAssist.SetUnderlineBrush(expenseDate_reminders, Brushes.DarkRed);
            //     HintAssist.SetHint(expenseDate_reminders, "Некоректно заполненое поле");
            // }

            if (titleReminder.Length < 3 || Regex.IsMatch(titleReminder, patternText, RegexOptions.IgnoreCase))
            {
                tittle_remiders.Foreground = Brushes.DarkRed;
                TextFieldAssist.SetUnderlineBrush(tittle_remiders, Brushes.DarkRed);
                HintAssist.SetHint(tittle_remiders, "Некоректно заполненое поле ");
                isCorect = false;
            }

            if (isCorect)
            {
                Reminder newReminder =
                    new Reminder(user.User_id, titleReminder.Trim(), ExpenseDate, comment_reminders.Text);

                db.Reminders.Add(newReminder);
                db.SaveChanges();

                SnackbarReminder.MessageQueue?.Enqueue("Запись добавлена!", null, null, null, false, true, TimeSpan.FromSeconds(3));
                expenseDate_reminders.Text = "";
                tittle_remiders.Text = "";
                comment_reminders.Text = "";

                MainWindow mainWindow = new MainWindow(user);
                mainWindow.Show();
                Close();

            }

        }

        private void
            PackIcon_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) // delete note from reminders
        {
            var cb = sender as PackIcon;
            var item = cb.DataContext;
            reminders_list.SelectedItem = item;
            DeleteReminder deleteReminder = new DeleteReminder(((Reminder)reminders_list.SelectedItem).Reminder_id);
            deleteReminder.Owner = this;
            deleteReminder.ShowDialog();

            MainWindow mainWindow = new MainWindow(user);
            mainWindow.Show();
            Close();
        }
    }
}