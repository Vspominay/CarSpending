using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarSpending
{
    public class Image
    {
        [Key]

        public int Image_id { get; set; }
        public string ImagePath { get; set; }
        public string ImageTitle { get; set; }

        public Image(){}

        public Image(string imagePath, string imageTitle = "")
        {
            ImagePath = imagePath;
            ImageTitle = imageTitle;
        }
    }
}
