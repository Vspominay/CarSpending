using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CarSpending
{
    public class User
    {
        [Key]
        public int User_id { get; set; }

        public string First_name { get; set; }

        public string Last_name { get; set; }

        public string Patronymic_name { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string Email {get; set; }


        public User(){}

        public User(string first_name, string login, string password, string email, string last_name = "", string patronymic_name = "")
        {
            First_name = first_name;
            Last_name = last_name;
            Patronymic_name = patronymic_name;
            Login = login;
            Password = password;
            Email = email;
        }
    }
}
