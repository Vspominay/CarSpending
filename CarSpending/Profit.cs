using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarSpending
{
    public class Profit
    {
        [Key]
        public int Profit_id { get; set; }
        public int User_id { get; set; }
        public int ProfitType_id { get; set; }
        public string Comment { get; set; }
        public double ProfitMargin_num { get; set; }
        public string Profit_date { get; set; }
        public double Mileage_num { get; set; }

        public Profit(int userId, int profitTypeId, double profitMargin, string profitDate, double mileage,
            string comment = "")
        {
            User_id = userId;
            ProfitType_id = profitTypeId;
            ProfitMargin_num = profitMargin;
            Profit_date = profitDate;
            Mileage_num = mileage;
            Comment = comment;
        }

        public Profit(){}

    }
}
