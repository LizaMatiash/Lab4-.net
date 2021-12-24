using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DAL.Entities;

namespace DAL.EF
{
    public class DataContext : DbContext
    {
        //public DataContext()
        //    : base("DbConnection")
        //{ }
        public DataContext()
            : base("DBContext")
        { }
        //public DataContext(string name)
        //    : base(name)
        //{ }
        public DbSet<User> User_s { get; set; }
        public DbSet<Friendship> Friendship_s { get; set; }
        public DbSet<Message> Message_s { get; set; }
    }

}
