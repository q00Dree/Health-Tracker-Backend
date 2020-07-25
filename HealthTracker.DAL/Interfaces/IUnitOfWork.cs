using HealthTracker.DAL.Entities;
using System;
using System.Threading.Tasks;

namespace HealthTracker.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Controller> Controllers { get; }
        IRepository<Sensor> Sensors { get; }
        bool IsDisposed { get; }
        Task SaveAsync();
        void Save();
    }
}
