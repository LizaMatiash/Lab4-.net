using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;//

namespace DAL.Interfaces
{
    public class UserRepository : IRepository<User>//Metodami repoz!!!!
    {
        private DataContext db;

        public UserRepository(DataContext context)
        {
            this.db = context;
        }

        public IEnumerable<User> GetAll()
        {
            return db.User_s;
        }

        public User Get(Guid id)
        {
            return db.User_s.Find(id);
        }

        public void Create(User user)
        {
            db.User_s.Add(user);
        }

        public IEnumerable<User> Where(Func<User, Boolean> predicate)
        {
            return db.User_s.Where(predicate).ToList();
        }

        public void Update(User user)
        {
            db.Entry(user).State = EntityState.Modified;
        }

        public void Delete(Guid id)
        {
            User user = db.User_s.Find(id);
            if (user != null)
                db.User_s.Remove(user);
        }
    }
}
