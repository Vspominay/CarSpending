using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarSpending
{
    public class Expense
    {
        [Key]
        public int Expenses_id { get; set; }
        public int Car_id { get; set; }
        public string Expense_date { get; set; }
        public double Mileage_num { get; set; }
        public double TotalCost { get; set; }
        public string Comment{ get; set; }
        public int Service_id { get; set; }
        public int Refill_id { get; set; }
        public string Location { get; set; }
        public ICollection<Favor> Favors { get; set; }

        public Expense()
        {
            Favors = new List<Favor>();
        }

        public Expense(int carId, string expanseDate, double mileage, double totalCost, 
            int serviceId, int refillId, string comment = "", string location = "")
        {
            Car_id = carId;
            Expense_date = expanseDate;
            Mileage_num = mileage;
            TotalCost = totalCost;
            Comment = comment;
            Service_id = serviceId;
            Refill_id = refillId;
            Location = location;
        }


    }
}
