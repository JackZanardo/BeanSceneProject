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
            var sitting = await _context.Sittings.FirstOrDefaultAsync(s => s.Id == id);
            return sitting;
        }
    }
}
