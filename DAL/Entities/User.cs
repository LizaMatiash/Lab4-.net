using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DAL.Entities
{
    public class User
    {
        public Guid ID { get; set; }

        public string Login { get; set; }
 
        public string Password { get; set; }

        public string Nickname { get; set; }

        //public DateTime RegistredTimeStamp { get; set; }
    }
}
