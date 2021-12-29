using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarSpending.ListboxItems
{
    public class FavorsStat
    {
        private ApplicationContext db;
        private DataClass dataClass;
        public string FavorStat { get; set; }

        public FavorsStat(){}

        public FavorsStat(string favorStat)
        {
            FavorStat = favorStat;
        }

        public string GetFavorStatistick(int service_id)
        {
            db = new ApplicationContext();
            dataClass = new DataClass();
            var result = "";

            var rowCollectionService = dataClass.selectQuery("select * from Services where Service_id = " + service_id).Rows;
            List<Service> serviceListFromId = dataClass.SelecServices(rowCollectionService);//favornum

            List<Favor> favorListFromServiceId = new List<Favor>();//favor_cost
            foreach (Favor favorItem in db.Favors)
            {
                if (serviceListFromId.Any(s => s.Favor_id == favorItem.Favor_id))
                {
                    favorListFromServiceId.Add(favorItem);
                }
            }

            List<favorType> favorTypeFromFavorId = new List<favorType>(); //favorName
            foreach (favorType favorType in db.FavorTypes)
            {
                if (favorListFromServiceId.Any(s => s.FavorType_id == favorType.FavorType_id))
                {
                    result += favorType.Favor_name + "&";
                }
            }

            var outputStr = "";
            var splittedStr = result.Split('&');
            if (splittedStr.Length > 2)
            {
                outputStr += $"{splittedStr[0]} (+{splittedStr.Length - 2})";
            }
            else
            {
                outputStr += splittedStr[0];
            }

            return outputStr;
        }
    }
}
