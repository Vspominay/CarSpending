using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarSpending
{
    public class Automation
    {
        [Key]

        public int Id_aut { get; set; }
        public int Car_id { get; set; }
        public string AutData { get; set; }

        public Automation()
        {

        }

        public Automation(int carId, string autData)
        {
            Car_id = carId;
            AutData = autData;
        }
    }
}
