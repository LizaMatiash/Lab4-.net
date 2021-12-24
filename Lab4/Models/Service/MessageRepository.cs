using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace Lab4.Models.Service
{
    public class MessageRepository : IRepository<Message>
    {
        private DataContext db;

        public MessageRepository(DataContext context)
        {
            this.db = context;
        }

        public IEnumerable<Message> GetAll()
        {
            return db.Message_s;
        }

        public Message Get(Guid id)
        {
            return db.Message_s.Find(id);
        }

        public void Create(Message message)
        {
            db.Message_s.Add(message);
        }

        public IEnumerable<Message> Where(Func<Message, Boolean> predicate)
        {
            return db.Message_s.Where(predicate).ToList();
        }

        public void Update(Message message)
        {
            db.Entry(message).State = EntityState.Modified;
        }

        public void Delete(Guid id)
        {
            Message message = db.Message_s.Find(id);
            if (message != null)
                db.Message_s.Remove(message);
        }
    }
}
