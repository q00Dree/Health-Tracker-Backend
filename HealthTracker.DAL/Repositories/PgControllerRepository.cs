using HealthTracker.DAL.Contexts;
using HealthTracker.DAL.Entities;
using HealthTracker.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace HealthTracker.DAL.Repositories
{
    public class PgControllerRepository : IRepository<Controller>
    {
        private readonly PgDbContext _context;
        public PgControllerRepository(PgDbContext context)
        {
            _context = context;
        }
        public void Add(Controller item)
        {
            _context.Controllers.Add(item);
        }
        public IEnumerable<Controller> GetAll()
        {
            return _context.Controllers.ToList();
        }

        public IEnumerable<Controller> GetAllIncluded()
        {
            return _context.Controllers.Include(n => n.Sensors).ToList();
        }

        public void Update(Controller item)
        {
            _context.Controllers.Update(item);
        }
    }
}
