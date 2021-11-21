using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarSpending
{
    public class Service
    {
        [Key]
        public int Service_id { get; set; }
        public int Favor_id { get; set; }
        public int Favor_num { get; set; }
        public Service(){}

        public Service(int serviceId, int favorId, int favorNum = 1)
        {
            Service_id = serviceId;
            Favor_num = favorNum;
            Favor_id = favorId;
        }
    }
}
