using BeanSceneProject.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Services
{
    public class SittingService
    {
        private readonly ApplicationDbContext _context;

        public SittingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Sitting> GetSittingAsync(int id)
        {
            var sitting = await _context.Sittings.Include(s => s.Reservations).FirstOrDefaultAsync(s => s.Id == id);
            return sitting;
        }

        public async Task<ICollection<Sitting>> GetSittingsAsync(DateTime openDate, bool isClosed)
        {
            var sittings = _context.Sittings.Include(s => s.SittingType).Include(s => s.Reservations).Where(s => s.Open.Date == openDate && s.IsClosed == isClosed);
            return await sittings.ToListAsync();
        }
    }
}
