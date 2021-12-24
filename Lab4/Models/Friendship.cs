using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4.Models
{
    public class Friendship
    {
        public Guid ID { get; set; }
        public Guid ListOwnerID { get; set; }
        public Guid IncludedPersonID { get; set; }
        public Guid RequestSenderID { get; set; }
        public Guid RequestReceiverID { get; set; }
        public bool RequestConfirmed { get; set; }
        //public DateTime SentTimeStamp { get; set; }
        //public DateTime ConfirmedTimeStamp { get; set; }
    }
}
