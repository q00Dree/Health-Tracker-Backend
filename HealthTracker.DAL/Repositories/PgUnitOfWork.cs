using HealthTracker.DAL.Contexts;
using HealthTracker.DAL.Entities;
using HealthTracker.DAL.Interfaces;
using HealthTracker.DAL.Repositories;
using System;
using System.Threading.Tasks;

namespace Qoollo.DAL.Repositories
{
    public class PgUnitOfWork : IUnitOfWork
    {
        public IRepository<Controller> Controllers { get; }
        public IRepository<Sensor> Sensors { get; }

        private readonly PgDbContext _context;
        public bool IsDisposed { get; private set; }

        public PgUnitOfWork(PgDbContext context)
        {
            _context = context;
            Sensors = new PgSensorRepository(_context);
            Controllers = new PgControllerRepository(_context);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }

            IsDisposed = true;
        }
    }
}
