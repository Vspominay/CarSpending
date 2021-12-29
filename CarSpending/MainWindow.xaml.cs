using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CarSpending.prompt;
using System.Data.Entity;
using System.Data.SQLite;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using CarSpending.ListboxItems;
using MaterialDesignThemes.Wpf;
using f = System.Windows.Forms;
using Wpf.CartesianChart.SolidColorChart;

namespace CarSpending
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private ApplicationContext db;
        private FavorsStat fs;
        public ObservableCollection<FuelType> FuelTypes { get; set; }

        public ObservableCollection<TypeOfserviceItem> TypeOfserviceItems { get; set; }
        public ObservableCollection<ProfitType> ProfitTypes { get; set; }
        public ObservableCollection<Reminder> RemindersList { get; set; }
        public ObservableCollection<Expense> ExpensesList { get; set; }

        private DataClass dataClass;



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

        private string typeIcon;

        public string TypeIcon
        {
            get => typeIcon;
            set
            {
                if (typeIcon != value)
                {
                    typeIcon = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName));
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

            PushIntoListbox(FuelTypes, typesOfFuel, db.FuelTypes); //push type services into listbox
            PushIntoListbox(ProfitTypes, typeOfProfit, db.ProfitTypes); //push profit types into listbox
            PushIntoListbox(RemindersList, reminders_list, db.Reminders); //push reminders into listbox
        }

        public void LoadDataIntoProfitsList(ListBox listOfProfits)
        {
            List<Profit> profitList = new List<Profit>();
            if (db.Profits.ToList().Count > 0)
            {
                try
                {
                    profitList = db.Profits.Where(pr=> pr.User_id == user.User_id).ToList();
                    listOfProfits.ItemsSource = new ObservableCollection<Profit>(profitList);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            else
            {
                MessageBox.Show("database is empty", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }
        public void LoadDataIntoExpensesList(ListBox listOfExpenses)
        {

            fs = new FavorsStat();
            DataTable dTable = new DataTable();
            List<Expense> testList = new List<Expense>();

            try
            {

                string sqlQuery = "SELECT * from Expenses WHERE Car_id = " + userCars[ComboBoxCars.SelectedIndex].Car_id;
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(sqlQuery, "Data Source=.\\spendingCar.db");
                adapter.Fill(dTable);


                if (dTable.Rows.Count > 0)
                {
                    TypeIcon = "Wrench";
                    int carId, serviceId, refillId, Expenses_id;
                    string expanseDate, comment, location, expenseName;
                    double mileage, totalCost;
                    for (int i = 0; i < dTable.Rows.Count; i++)
                    {
                        Expenses_id = Convert.ToInt32(dTable.Rows[i].ItemArray[0]);
                        carId = Convert.ToInt32(dTable.Rows[i].ItemArray[1]);
                        expanseDate = dTable.Rows[i].ItemArray[2].ToString();
                        mileage = Convert.ToDouble(dTable.Rows[i].ItemArray[3]);
                        totalCost = Convert.ToDouble(dTable.Rows[i].ItemArray[4]);
                        try
                        {
                            serviceId = Convert.ToInt32(dTable.Rows[i].ItemArray[6]);
                        }
                        catch (Exception e)
                        {
                            serviceId = -1;
                        }

                        try
                        {
                            refillId = Convert.ToInt32(dTable.Rows[i].ItemArray[7]);
                        }
                        catch (Exception e)
                        {
                            refillId = -1;
                        }

                        comment = dTable.Rows[i].ItemArray[5].ToString();
                        location = dTable.Rows[i].ItemArray[8].ToString();

                        expenseName = serviceId != -1 ? fs.GetFavorStatistick(serviceId) : "Заправка";

                        var exp = new Expense
                        {
                            Car_id = carId,
                            Expense_date = expanseDate,
                            Mileage_num = mileage,
                            TotalCost = totalCost,
                            Service_id = serviceId,
                            Refill_id = refillId,
                            Comment = comment,
                            Location = location,
                            Expenses_id = Expenses_id,
                            ExpenseName = expenseName
                        };
                        testList.Add(exp);
                    }
                }
                else
                    MessageBox.Show("Database is empty");
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }


            FilterExpense(ref listOfExpenses, new ObservableCollection<Expense>(), new List<Expense>(), testList,listOfRefills,listOfService);

        }

        public void FilterExpense(ref ListBox listbox,ObservableCollection<Expense> observable ,List<Expense> tempList,List<Expense> testList, ListBox listRef, ListBox listSer)
        {
            if (listbox == listRef)
            {
                foreach (var testItem in testList)
                {
                    if (testItem.Service_id == -1)
                    {
                        tempList.Add(testItem);
                    }
                }

                listbox.ItemsSource = new ObservableCollection<Expense>(tempList);
            }
            else if (listbox == listSer)
            {
                foreach (var testItem in testList)
                {
                    if (testItem.Refill_id == -1)
                    {
                        tempList.Add(testItem);
                    }
                }
                listbox.ItemsSource = new ObservableCollection<Expense>(tempList);
            }
            else
            {

                listbox.ItemsSource = new ObservableCollection<Expense>(testList);
            }
        }


        public void PushIntoListbox<T>(ObservableCollection<T> observableCollection, ListBox listBox,DbSet dbSet)
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
                "Просмотр статистики для автомобиля " + userCars[ComboBoxCars.SelectedIndex].CarBrand;

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

        public void MakeAutomation(double amountLiter, double carMileage)
        {
            dataClass = new DataClass();
            double tankVolume = userCars[ComboBoxCars.SelectedIndex].TankVolume_num;
            var beforeRefillList = db.Refills.OrderByDescending(r => r.Refill_id).Take(1).ToList()[0];

            if (beforeRefillList.FullTank_status == 1)
            {
                double beforeRefillLiters = beforeRefillList.AmountLiter_num;
                double tempLiters = tankVolume - amountLiter;

                var tempBeforeMileage = dataClass.selectQuery("SELECT * from expenses where Refill_id = " + beforeRefillList.Refill_id).Rows;

                var beforeMileage = dataClass.SelecExpenses(tempBeforeMileage);

                double resultValue = Math.Round(amountLiter / (carMileage - beforeMileage[0].Mileage_num) * 100, digits: 2);

                if (db.Automations.ToList().Any(au => au.Car_id == userCars[ComboBoxCars.SelectedIndex].Car_id && au.AutData != ""))
                {
                    var selectAut = db.Automations.ToList().FirstOrDefault(aut => aut.Car_id == userCars[ComboBoxCars.SelectedIndex].Car_id);
                    selectAut.AutData += " " + resultValue;
                }
                else
                {
                    Automation newAut = new Automation(userCars[ComboBoxCars.SelectedIndex].Car_id, resultValue+"");
                    db.Automations.Add(newAut);
                }
            }
           
            db.SaveChanges();

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



            validationCost(literCost_refill,ref literCost,ref isCorect, patternText);
           

            validationCost(amountLiters_refill,ref amountLiter,ref isCorect, patternText);
            

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

            if (isCorect)
            {
                if (fullTank == 1)
                {
                    MakeAutomation(amountLiter, carMileage);
                }

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
           
            ValidationMileage(mileage_service, ref carMileage, ref isCorect);
            

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
           

            validationCost(literCost_profit,ref profitMargin,ref isCorect,patternText);
           
            
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

            ValidationDate(expenseDate_profit, ref expenseDate, ref isCorect);
           
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
            DeleteReminder deleteReminder = new DeleteReminder(((Reminder)reminders_list.SelectedItem).Reminder_id,user);
            deleteReminder.Owner = this;
            deleteReminder.ShowDialog();

           
        }


        private void deleteItemFromList_click(object sender, MouseButtonEventArgs e)
        {
            deteItemExpenses(listOfExpenses, sender, SnackBarExpense);
        }

        public void deteItemExpenses(ListBox listOfExp, object sender,Snackbar snakbar)
        {
            dataClass = new DataClass();
            var cb = sender as StackPanel;
            var item = cb.DataContext;
            listOfExp.SelectedItem = item;
            Expense selectExpense = (Expense)(listOfExp.SelectedItem);
            int tempCountList = listOfExp.Items.Count;
            DeleteExpense delete = new DeleteExpense(dataClass, selectExpense);
            delete.Owner = this;
            delete.ShowDialog();
            LoadDataIntoExpensesList(listOfExp);
            if (tempCountList != listOfExp.Items.Count)
            {
                snakbar.MessageQueue?.Enqueue("Запись удалена!", null, null, null, false, true,
                    TimeSpan.FromSeconds(3));
            }
        }

        public void PushElementsIntopTopCost()
        {
            var topFavorsCost = db.Favors.OrderByDescending(f => f.FavorCost_num).Take(5).ToList();
            List<favorType> favorTypes = new List<favorType>();

            for (int i = 0; i < topFavorsCost.Count; i++)
            {
                favorTypes.Add(db.FavorTypes.ToList().FirstOrDefault(ft=> ft.FavorType_id == topFavorsCost[i].FavorType_id));
            }
            
            var TypeOfserviceItems = new ObservableCollection<TypeOfserviceItem>();
            for (int i = 0; i < topFavorsCost.Count; i++)
            {
                TypeOfserviceItems.Add(new TypeOfserviceItem
                {
                    FavorCost_num = topFavorsCost[i].FavorCost_num,
                    Favor_name = favorTypes[i].Favor_name,
                });
            }
            topCostList.ItemsSource = TypeOfserviceItems;
            topCostList_print.ItemsSource = TypeOfserviceItems;

        }
        private void TabItem_PreviewMouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            LoadDataIntoExpensesList(listOfExpenses);
            PushElementsIntopTopCost();
        }



        private void selectRadiusDateOpen_click(object sender, MouseButtonEventArgs e)
        {
            SelectDateRadius selectDate = new SelectDateRadius(dateRadius_exp,
                listOfExpenses,
                countNote_exp,
                totalExpesns_exp,
                dayExpesns_exp,
                totalMileage_exp,
                dayMilage_exp,
                listOfRefills,
                listOfService,
                null,
                TitleExpOrProf,
                userCars[ComboBoxCars.SelectedIndex],
                user);

            selectDate.Owner = this;
            selectDate.ShowDialog();
            dateRadius_exp_print.Text = dateRadius_exp.Text;
            totalExpesns_exp_print.Text = totalExpesns_exp.Text;
            dayExpesns_exp_print.Text = dayExpesns_exp.Text;
            totalMileage_exp_print.Text = totalMileage_exp.Text;
            dayMilage_exp_print.Text = dayMilage_exp.Text;


            cardAboutAllReport.Visibility = Visibility.Visible;
            cardInfoAboutItemExpense.Visibility = Visibility.Hidden;
        }



        private void SearchNotesinExpenses(object sender, MouseButtonEventArgs e)//search notes in expenses used date and mileage
        {
            SearchData search = new SearchData(listOfExpenses,listOfProfits, userCars[ComboBoxCars.SelectedIndex],user);
            search.Owner = this;
            search.ShowDialog();

        }

        private void sorttedExpenses_click(object sender, MouseButtonEventArgs e)
        {
            SortWindow sortWindow = new SortWindow(listOfExpenses,
                listOfRefills,
                listOfService,
                null,
                userCars[ComboBoxCars.SelectedIndex],user);
            sortWindow.Owner = this;
            sortWindow.ShowDialog();
        }

        public void PushGeneralData(List<Expense> filtersDateList)
        {
            totalCost_expense.Text = filtersDateList[0].TotalCost + "";
            Mileage_expense.Text = filtersDateList[0].Mileage_num + "";
            Date_expense.Text = filtersDateList[0].Expense_date + "";
            location_expense.Text = filtersDateList[0].Location + "";
            comment_expense.Text = filtersDateList[0].Comment + "";
            cardAboutAllReport.Visibility = Visibility.Hidden;
            cardInfoAboutItemExpense.Visibility = Visibility.Visible;
        }

        public void PushRefilsData(List<Expense> filtersDateList)
        {
            gasGrid.Visibility = Visibility.Visible;
            ServiceGrid.Visibility = Visibility.Hidden;
            int idRefil = filtersDateList[0].Refill_id;
            Refill refilItem = db.Refills.FirstOrDefault(rItem => rItem.Refill_id == idRefil);
            string typeOfFuel = db.FuelTypes.FirstOrDefault(typeFuel => typeFuel.Fuel_id == refilItem.Fuel_id)?.Fuel_title;
            typeOfFuel_expense.Text = typeOfFuel;
            isFulTank_expense.Text = refilItem.FullTank_status == 1 ? "Да" : "Нет";
            oneLitr_expense.Text = refilItem.LiterCost_num + "";
            countLiters_expense.Text = refilItem.AmountLiter_num + "";
        }
        public void PushServiceData(List<Expense> filtersDateList)
        {
            gasGrid.Visibility = Visibility.Hidden;
            ServiceGrid.Visibility = Visibility.Visible;
            int serviceId = filtersDateList[0].Service_id;
            var rowCollectionService = dataClass.selectQuery("select * from Services where Service_id = " + serviceId).Rows;
            List<Service> serviceListFromId = dataClass.SelecServices(rowCollectionService);//favornum

            List<Favor> favorListFromServiceId = new List<Favor>();//favor_cost
            foreach (Favor favorItem in db.Favors)
            {
                if (serviceListFromId.Any(s => s.Favor_id == favorItem.Favor_id))
                {
                    favorListFromServiceId.Add(favorItem);
                }
            }

            List<favorType> favorTypeFromFavorId = new List<favorType>(); //favorName
            foreach (favorType favorType in db.FavorTypes)
            {
                if (favorListFromServiceId.Any(s => s.FavorType_id == favorType.FavorType_id))
                {
                    favorTypeFromFavorId.Add(favorType);
                }
            }

            TypeOfserviceItems = new ObservableCollection<TypeOfserviceItem>();
            for (int i = 0; i < favorTypeFromFavorId.Count; i++)
            {
                TypeOfserviceItems.Add(new TypeOfserviceItem(favorTypeFromFavorId[i].Favor_name,
                    favorListFromServiceId[i].FavorCost_num, serviceListFromId[i].Favor_num));
            }

            listOfFavors.ItemsSource = TypeOfserviceItems;
        }
        private void ShowInformationAboutExpense(object sender, MouseButtonEventArgs e)
        {
            ShowDetailsExpense(listOfExpenses, sender);
        }

        public void ShowDetailsExpense(ListBox listOfExpense,object sender)
        {
            db = new ApplicationContext();
            var cb = sender as StackPanel;
            var item = cb.DataContext;
            listOfExpense.SelectedItem = item;
            dataClass = new DataClass();
            int idExpense = ((Expense)listOfExpense.SelectedItem).Expenses_id;
            var itemCost = dataClass.selectQuery("select * from Expenses where Expenses_id = " + idExpense).Rows;
            var filtersDateList = dataClass.SelecExpenses(itemCost);
            PushGeneralData(filtersDateList);

            if (filtersDateList[0].Service_id == -1)
            {
                PushRefilsData(filtersDateList);
            }
            else
            {
                PushServiceData(filtersDateList);
            }
        }

        private void refillsTabItem_click(object sender, MouseButtonEventArgs e)
        {
            LoadDataIntoExpensesList(listOfRefills);
        }

        private void servicelistTabItem_click(object sender, MouseButtonEventArgs e)
        {
            LoadDataIntoExpensesList(listOfService);
        }

        private void SearchNotesinRefills(object sender, MouseButtonEventArgs e)
        {
            SearchData search = new SearchData(listOfRefills, listOfProfits, userCars[ComboBoxCars.SelectedIndex], user);
            search.Owner = this;
            search.ShowDialog();
        }

        private void selectRadiusDateOpenRefil_click(object sender, MouseButtonEventArgs e)
        {
            SelectDateRadius selectDate = new SelectDateRadius(dateRadius_exp,
                listOfRefills,
                countNote_exp,
                totalExpesns_exp,
                dayExpesns_exp,
                totalMileage_exp,
                dayMilage_exp,
                listOfRefills,
                listOfService,
                null,
                TitleExpOrProf,
                userCars[ComboBoxCars.SelectedIndex],
                user);
            selectDate.Owner = this;
            selectDate.ShowDialog();
            cardAboutAllReport.Visibility = Visibility.Visible;
            cardInfoAboutItemExpense.Visibility = Visibility.Hidden;
        }

        private void SearchNotesinServices(object sender, MouseButtonEventArgs e)
        {
            SearchData search = new SearchData(listOfService, listOfProfits, userCars[ComboBoxCars.SelectedIndex], user);
            search.Owner = this;
            search.ShowDialog();
        }

        private void selectRadiusDateOpenService_click(object sender, MouseButtonEventArgs e)
        {
            SelectDateRadius selectDate = new SelectDateRadius(dateRadius_exp,
                listOfService,
                countNote_exp,
                totalExpesns_exp,
                dayExpesns_exp,
                totalMileage_exp,
                dayMilage_exp,
                listOfRefills,
                listOfService,
                null,
                TitleExpOrProf,
                userCars[ComboBoxCars.SelectedIndex],
                user);
            selectDate.Owner = this;
            selectDate.ShowDialog();
            cardAboutAllReport.Visibility = Visibility.Visible;
            cardInfoAboutItemExpense.Visibility = Visibility.Hidden;
        }

        private void sorttedRefils_click(object sender, MouseButtonEventArgs e)
        {
            SortWindow sortWindow = new SortWindow(listOfRefills,
                listOfRefills,
                listOfService,
                null,
                userCars[ComboBoxCars.SelectedIndex],user);
            sortWindow.Owner = this;
            sortWindow.ShowDialog();
        }

        private void sorttedService_click(object sender, MouseButtonEventArgs e)
        {
            SortWindow sortWindow = new SortWindow(listOfService,
                listOfRefills,
                listOfService,
                null,
                userCars[ComboBoxCars.SelectedIndex],user);
            sortWindow.Owner = this;
            sortWindow.ShowDialog();
        }

        private void ShowInformationAboutReffil(object sender, MouseButtonEventArgs e)
        {
            ShowDetailsExpense(listOfRefills,sender);
        }

        private void ShowInformationAboutService(object sender, MouseButtonEventArgs e)
        {
            ShowDetailsExpense(listOfService, sender);
        }

        private void deleteItemFromListService_click(object sender, MouseButtonEventArgs e)
        {
            deteItemExpenses(listOfService, sender, SnackBarService);
        }

        private void deleteItemFromListRefills_click(object sender, MouseButtonEventArgs e)
        {
            deteItemExpenses(listOfRefills, sender, SnackBarRefill);
        }

        private void profitTabItem_click(object sender, MouseButtonEventArgs e)
        {
            LoadDataIntoProfitsList(listOfProfits);
        }

        private void deleteItemFromListProfits_click(object sender, MouseButtonEventArgs e)
        {
            DeleteItemProfit(listOfProfits,sender);
        }

        private void DeleteItemProfit(ListBox listBox, object sender)
        {
            dataClass = new DataClass();
            var cb = sender as StackPanel;
            var item = cb.DataContext;
            listBox.SelectedItem = item;
            Profit selectProfit = (Profit)(listBox.SelectedItem);
            int tempCountList = listBox.Items.Count;
            DeleteProfit delete = new DeleteProfit(dataClass, selectProfit);
            delete.Owner = this;
            delete.ShowDialog();
            LoadDataIntoProfitsList(listOfProfits);
            if (tempCountList != listBox.Items.Count)
            {
                SnackBarProfit.MessageQueue?.Enqueue("Запись удалена!", null, null, null, false, true,
                    TimeSpan.FromSeconds(3));
            }
        }

        private void SearchProfit(object sender, MouseButtonEventArgs e)
        {
            SearchData search = new SearchData(listOfProfits, listOfProfits, userCars[ComboBoxCars.SelectedIndex],user);
            search.Owner = this;
            search.ShowDialog();
        }

        private void sorttedProfits_click(object sender, MouseButtonEventArgs e)
        {
            SortWindow sortWindow = new SortWindow(listOfService,
                listOfRefills,
                listOfService,
                listOfProfits,
                userCars[ComboBoxCars.SelectedIndex],user);
            sortWindow.Owner = this;
            sortWindow.ShowDialog();
        }

        private void selectRadiusDateOpenProfit_click(object sender, MouseButtonEventArgs e)
        {
            SelectDateRadius selectDate = new SelectDateRadius(dateRadius_exp,
                listOfService,
                countNote_exp,
                totalExpesns_exp,
                dayExpesns_exp,
                totalMileage_exp,
                dayMilage_exp,
                listOfRefills,
                listOfService,
                listOfProfits,
                TitleExpOrProf,
                userCars[ComboBoxCars.SelectedIndex],
                user);
            selectDate.Owner = this;
            selectDate.ShowDialog();
            cardAboutAllReport.Visibility = Visibility.Visible;
            cardInfoAboutItemExpense.Visibility = Visibility.Hidden;
        }

        private void UIElement_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MonthStatistic monthStatistic = new MonthStatistic(userCars[ComboBoxCars.SelectedIndex]);
            monthStatistic.Owner = this;
            monthStatistic.ShowDialog();
        }

        private void makeReport(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.IsEnabled = false;
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintVisual(cardAboutAllReport_print, "Общий отчёт за период");
                }
            }
            finally
            {
                this.IsEnabled = true;
            }
        }

        private void makeReportExpens(object sender, MouseButtonEventArgs e)
        {
            if (gasGrid.Visibility == Visibility.Visible)
            {
                gasGrid_print.Visibility = Visibility.Visible;
                ServiceGrid_print.Visibility = Visibility.Hidden;
            }
            else
            {
                gasGrid_print.Visibility = Visibility.Hidden;
                ServiceGrid_print.Visibility = Visibility.Visible;
            }

            totalCost_expense_print.Text = totalCost_expense.Text;
            Mileage_expense_print.Text = Mileage_expense.Text;
            Date_expense_print.Text = Date_expense.Text;
            location_expense_print.Text = location_expense.Text;
            countLiters_expense_print.Text = countLiters_expense.Text;
            oneLitr_expense_print.Text = oneLitr_expense.Text;
            listOfFavors_print.ItemsSource = listOfFavors.ItemsSource;
            comment_expense_print.Text = comment_expense.Text;
            isFulTank_expense_print.Text = isFulTank_expense.Text;

            try
            {
                this.IsEnabled = false;
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintVisual(reportExpense_print, "Общий отчёт за период");
                }
            }
            finally
            {
                this.IsEnabled = true;
            }
        }

        private void ShowGraphic(object sender, MouseButtonEventArgs e)
        {
            var warning = false;
            SolidColorExample drawGraph = new SolidColorExample(userCars[ComboBoxCars.SelectedIndex],ref warning);
            if (!warning)
            {
                drawGraph.Owner = this;
                drawGraph.ShowDialog();
            }

        }

        private void openQuestWind_click(object sender, MouseButtonEventArgs e)
        {
            UserQuestions userq = new UserQuestions();
            userq.Owner = this;
            userq.ShowDialog();
        }

        private void changeData_click(object sender, RoutedEventArgs e)
        {
            ChangeICarInfo changeCar = new ChangeICarInfo(userCars[ComboBoxCars.SelectedIndex],user);
            changeCar.Owner = this;
            changeCar.ShowDialog();
        }
    }
}