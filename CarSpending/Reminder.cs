using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarSpending
{
    public class Reminder
    {
        [Key]

        public int Reminder_id { get; set; }
        public int User_id { get; set; }

        public string Reminder_title { get; set; }
        public string Comment { get; set; }
        public string Reminder_date { get; set; }


        public Reminder(){}

        public Reminder(int userId, string reminderTitle,string reminderDate, string comment)
        {
            User_id = userId;
            Reminder_title = reminderTitle;
            Comment = comment;
            Reminder_date = reminderDate;
        }
    }
}
