using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarSpending
{
    public class Refill
    {
        [Key]
        public int Refill_id { get; set; }
        public int Fuel_id { get; set; }
        public double LiterCost_num { get; set; }
        public double AmountLiter_num { get; set; }
        public int FullTank_status { get; set; }

        public Refill(){}

        public Refill(int fuelId, double literCost, double amouAnountLiterNum, int fullTank)
        {
            Fuel_id = fuelId;
            LiterCost_num = literCost;
            AmountLiter_num = amouAnountLiterNum;
            FullTank_status = fullTank;
        }
    }
}
