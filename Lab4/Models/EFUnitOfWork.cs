using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private DataContext db;
        private UserRepository userRepository;
        private FriendshipRepository friendshipRepository;
        private MessageRepository messageRepository;

        public EFUnitOfWork()
        {
            db = new DataContext();
        }
        public IRepository<User> User_s
        {
            get
            {
                if (userRepository == null)
                    userRepository = new UserRepository(db);
                return userRepository;
            }
        }

        public IRepository<Friendship> Friendship_s
        {
            get
            {
                if (friendshipRepository == null)
                    friendshipRepository = new FriendshipRepository(db);
                return friendshipRepository;
            }
        }

        public IRepository<Message> Message_s
        {
            get
            {
                if (messageRepository == null)
                    messageRepository = new MessageRepository(db);
                return messageRepository;
            }
        }

        public void Save()
        {
            db.SaveChanges();
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
