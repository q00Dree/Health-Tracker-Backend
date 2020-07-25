using Qoollo.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.DAL.Interfaces
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
