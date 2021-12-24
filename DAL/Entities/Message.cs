using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DAL.Entities
{
    public class Message
    {
        public Guid ID { get; set; }
        public Guid SenderID { get; set; }
        public Guid ReceiverID { get; set; }
        public string MessageBody { get; set; }
        public DateTime SentTimeStamp { get; set; }

        //public DateTime WasReadTimeStamp { get; set; }
    }
}
