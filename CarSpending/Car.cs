using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarSpending
{
    public class Car
    {
        [Key]
        public int Car_id { get; set; }
        public string Vin_num { get; set; }
        public double Mileage_num { get; set; }
        public double TankVolume_num { get; set; }
        public int User_id { get; set; }
        public string CarBrand { get; set; }
        public int Image_id { get; set; }

        public Car(){}

        public Car(double mileageNum, double tankVolume, int userId,
            string carBrand, string VinNum = "", int imageId = 0)
        {
            Vin_num = VinNum;
            Mileage_num = mileageNum;
            TankVolume_num = tankVolume;
            User_id = userId;
            CarBrand = carBrand;
            Image_id = imageId;
        }
    }
}
