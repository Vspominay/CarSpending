using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarSpending
{
    public class Favor
    {
        [Key]

        public int Favor_id { get; set; }
        public int FavorType_id { get; set; }
        public double FavorCost_num { get; set; }

        public Favor(){}
        public ICollection<Expense> Expenses{ get; set; }

        public Favor(int favorTypeId, double favorCost)
        {
            FavorType_id = favorTypeId;
            FavorCost_num = favorCost;
        }

        public Favor(int favorTypeId, ICollection<Expense> expenses, double favorCost)
        {
            FavorType_id = favorTypeId;
            FavorCost_num = favorCost;
            Expenses = expenses;
        }

    }
}
