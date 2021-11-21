using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace CarSpending
{
    class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<FuelType> FuelTypes { get; set; }
        public DbSet<Refill> Refills { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<favorType> FavorTypes { get; set; }
        public DbSet<Favor> Favors { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet <ProfitType> ProfitTypes { get; set; }
        public  DbSet <Profit> Profits { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public ApplicationContext() : base("DefaultConnection") {}
    }
}
