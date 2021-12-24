using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;//

namespace DAL.Interfaces
{

    public class FriendshipRepository : IRepository<Friendship>
    {
        private DataContext db;

        public FriendshipRepository(DataContext context)
        {
            this.db = context;
        }

        public IEnumerable<Friendship> GetAll()
        {
            return db.Friendship_s;
        }

        public Friendship Get(Guid id)
        {
            return db.Friendship_s.Find(id);
        }

        public void Create(Friendship friend)
        {
            db.Friendship_s.Add(friend);
        }

        public IEnumerable<Friendship> Where(Func<Friendship, Boolean> predicate)
        {
            return db.Friendship_s.Where(predicate).ToList();
        }

        public void Update(Friendship friend)
        {
            db.Entry(friend).State = EntityState.Modified;
        }

        public void Delete(Guid id)
        {
            Friendship friend = db.Friendship_s.Find(id);
            if (friend != null)
                db.Friendship_s.Remove(friend);
        }
    }
}
