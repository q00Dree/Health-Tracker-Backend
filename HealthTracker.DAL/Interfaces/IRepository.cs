using System.Collections.Generic;

namespace HealthTracker.DAL.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        void Add(T item);
        void Update(T item);
        IEnumerable<T> GetAllIncluded();
    }
}
