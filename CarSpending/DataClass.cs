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
    }
}
