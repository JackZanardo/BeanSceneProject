using BeanSceneProject.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Services
{
    public class ReservationService
    {
        private readonly ApplicationDbContext _context;

        public ReservationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Reservation> GetReservations()
        {
            var reservations = _context.Reservations
                .Include(r => r.Person)
                .Include(r => r.ReservationOrigin)
                .Include(r => r.Tables)
                .ThenInclude(t => t.Area);
            return reservations;
        }

        public IQueryable<Reservation> GetReservations(int sittingId)
        {
            var reservations = _context.Reservations
                .Include(r => r.Person)
                .Include(r => r.ReservationOrigin)
                .Include(r => r.Tables)
                .ThenInclude(t => t.Area)
                .Where(r => r.SittingId == sittingId);
            return reservations;
        }
    }
}
