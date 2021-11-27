using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarSpending.ListboxItems
{
     public class MonthCost
    {
        public string MonthName { get; set; }
        public double TotalCost { get; set; }

        public MonthCost(){}

        public MonthCost(string monthName, double totalCost)
        {
            MonthName = monthName;
            TotalCost = totalCost;
        }

    }
}
