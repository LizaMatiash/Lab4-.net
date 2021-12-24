using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4.Models.Service
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> User_s { get; }
        IRepository<Friendship> Friendship_s { get; }
        IRepository<Message> Message_s { get; }
        void Save();
    }
}
