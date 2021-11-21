using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarSpending
{
    public class favorType
    {
        [Key]
        public int FavorType_id { get; set; }
        public string Favor_name { get; set; }

        public favorType(){}

        public favorType(string favorName)
        {
            Favor_name = favorName;
        }
    }
}
