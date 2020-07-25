using Microsoft.EntityFrameworkCore;
using Qoollo.DAL.Contexts;
using Qoollo.DAL.Entities;
using Qoollo.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.DAL.Repositories
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
