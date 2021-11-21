using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarSpending.ListboxItems
{
    public class TypeOfserviceItem
    {
        public string Favor_name { get; set; }
        public double FavorCost_num { get; set; }
        public int Favor_num { get; set; }

        public TypeOfserviceItem(){}
        public TypeOfserviceItem(string favorName, double favorCost, int favorNum)
        {
            FavorCost_num = favorCost;
            Favor_name = favorName;
            Favor_num = favorNum;
        }
    }
}
