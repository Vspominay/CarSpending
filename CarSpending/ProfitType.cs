using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarSpending
{
   public class ProfitType
    {
        [Key]
        public int ProfitType_id { get; set; }
        public string ProfitType_title { get; set; }

        public ProfitType() { }

        public ProfitType(string ProfitType_title)
        {
            this.ProfitType_title = ProfitType_title;
        }
    }
}
