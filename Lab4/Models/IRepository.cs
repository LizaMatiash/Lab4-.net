using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T Get(Guid id);
        void Create(T item);
        IEnumerable<T> Where(Func<T, Boolean> predicate);
        void Update(T item);
        void Delete(Guid id);
    }
}
