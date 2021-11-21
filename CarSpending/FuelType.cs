using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarSpending
{
    public class FuelType
    {
        [Key]
        public int Fuel_id { get; set; }
        public string Fuel_title { get; set; }

        public FuelType(){}

        public FuelType(string Fuel_title)
        {
            this.Fuel_title = Fuel_title;
        }
    }
}
