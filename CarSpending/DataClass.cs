using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CarSpending
{
    public class DataClass
    {
        private SQLiteConnection sqlite;

        public DataClass()
        {
            //This part killed me in the beginning.  I was specifying "DataSource"
            //instead of "Data Source"
            sqlite = new SQLiteConnection("Data Source=.\\spendingCar.db");

        }

        public DataTable selectQuery(string query)
        {
            SQLiteDataAdapter ad;
            DataTable dt = new DataTable();

            try
            {
                SQLiteCommand cmd;
                sqlite.Open(); //Initiate connection to the db
                cmd = sqlite.CreateCommand();
                cmd.CommandText = query; //set the passed query
                ad = new SQLiteDataAdapter(cmd);
                ad.Fill(dt); //fill the datasource
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex+"");
            }

            sqlite.Close();
            return dt;
        }

        public List<Expense> SelecExpenses(DataRowCollection dTable)
        {
            List<Expense> testList = new List<Expense>();
            if (dTable.Count > 0)
            {
                int carId, serviceId, refillId, Expenses_id;
                string expanseDate, comment, location;
                double mileage, totalCost;
                for (int i = 0; i < dTable.Count; i++)
                {
                    Expenses_id = Convert.ToInt32(dTable[i].ItemArray[0]);
                    carId = Convert.ToInt32(dTable[i].ItemArray[1]);
                    expanseDate = dTable[i].ItemArray[2].ToString();
                    mileage = Convert.ToDouble(dTable[i].ItemArray[3]);
                    totalCost = Convert.ToDouble(dTable[i].ItemArray[4]);
                    try
                    {
                        serviceId = Convert.ToInt32(dTable[i].ItemArray[6]);
                    }
                    catch (Exception e)
                    {
                        serviceId = -1;
                    }

                    try
                    {
                        refillId = Convert.ToInt32(dTable[i].ItemArray[7]);
                    }
                    catch (Exception e)
                    {
                        refillId = -1;
                    }

                    comment = dTable[i].ItemArray[5].ToString();
                    location = dTable[i].ItemArray[8].ToString();

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
                        Expenses_id = Expenses_id
                    };

                    testList.Add(exp);
                }
            }
            else
                MessageBox.Show("Database is empty");

            return testList;
        }

        public List<Service> SelecServices(DataRowCollection dTable)
        {
            List<Service> testList = new List<Service>();
            if (dTable.Count > 0)
            {
                
                int serviceId, favorId, favorNum;
                
                for (int i = 0; i < dTable.Count; i++)
                {
                    serviceId = Convert.ToInt32(dTable[i].ItemArray[0]);
                    favorId = Convert.ToInt32(dTable[i].ItemArray[1]);
                    favorNum = Convert.ToInt32(dTable[i].ItemArray[2]);
                    
                    var ser = new Service
                    {
                        Service_id = serviceId,
                        Favor_id = favorId,
                        Favor_num = favorNum
                    };

                    testList.Add(ser);
                }
            }
            else
                MessageBox.Show("Database is empty");

            return testList;
        }
    }
}
