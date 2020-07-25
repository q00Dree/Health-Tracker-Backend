using HealthTracker.DAL.Contexts;
using HealthTracker.DAL.Entities;
using HealthTracker.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace HealthTracker.DAL.Repositories
{
    public class PgSensorRepository : IRepository<Sensor>
    {
        private readonly PgDbContext _context;
        public PgSensorRepository(PgDbContext context)
        {
            _context = context;
        }
        public void Add(Sensor item)
        {
            _context.Sensors.Add(item);
        }

        public IEnumerable<Sensor> GetAll()
        {
            return _context.Sensors.ToList();
        }

        public IEnumerable<Sensor> GetAllIncluded()
        {
            return _context.Sensors.Include(n => n.Controller).ToList();
        }

        public void Update(Sensor item)
        {
            _context.Sensors.Update(item);
        }
    }
}
