using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Lab4.Models
{
    public class Message
    {
        public Guid ID { get; set; }
        public Guid SenderID { get; set; }
        public Guid ReceiverID { get; set; }
        [Required(ErrorMessage = "Поле должно быть установлено")]
        [Display(Name = "Повідомлення")] 
        public string MessageBody { get; set; }
        public DateTime SentTimeStamp { get; set; }
        //public DateTime WasReadTimeStamp { get; set; }
    }
}
