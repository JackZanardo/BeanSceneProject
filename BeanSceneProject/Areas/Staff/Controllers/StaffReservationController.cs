using BeanSceneProject.Data;
using BeanSceneProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Areas.Staff.Controllers
{
    [Area("Staff"), Authorize(Roles = "Admin, Staff")]
    public class StaffReservationController : StaffAreaBaseController
    {
        private readonly SittingService _sittingService;
        public StaffReservationController(ApplicationDbContext context, SittingService sittingService) : base(context) 
        {
            _sittingService = sittingService;
        }

        public async Task<IActionResult> SittingIndex()
        {
            var sittings = _context.Sittings
                .Where(s => !s.IsClosed && s.Close >= DateTime.Now)
                .Include(s => s.SittingType)
                .Include(s => s.Restaurant)
                .Include(s => s.Reservations)
                .ThenInclude(r => r.ReservationOrigin);
            return View(await sittings.ToListAsync());
        }

        public async Task<IActionResult> Index(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var reservations = _context.Reservations
                .Where(r => r.SittingId == id)
                .Include(r => r.Person)
                .Include(r => r.ReservationOrigin)
                .Include(r => r.Sitting)
                .ThenInclude(s => s.Restaurant)
                .ThenInclude(r => r.Areas)
                .ThenInclude(a => a.Tables);
            var m = new Models.StaffReservation.Index
            {
                SittingId = id,
                Reservations = await reservations.ToListAsync()
            };
            return View(m);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var reservation = await _context.Reservations
                .Include(r => r.Person)
                .Include(r => r.ReservationOrigin)
                .Include(r => r.Sitting)
                .ThenInclude(s => s.Restaurant)
                .ThenInclude(r => r.Areas)
                .ThenInclude(a => a.Tables)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }
            return View(reservation);
        }

        public IActionResult Create(int? id)
        {
            var m = new Models.StaffReservation.Create
            {
                SittingId = id,
                Status = 1,
                ReservationOrigins = new SelectList(_context.ReservationOrigins.ToArray(), nameof(ReservationOrigin.Id), nameof(ReservationOrigin.Name)),
                Areas = _context.Areas.ToList(),
                Tables = new MultiSelectList(_context.Tables.ToArray(), nameof(Table.Id), nameof(Table.Name))
            };
            return View(m);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.StaffReservation.Create m)
        {
            if (ModelState.IsValid)
            {
                var sitting = await _context.Sittings.FirstOrDefaultAsync(s => s.Id == (int)m.SittingId);
                var r = new Reservation
                {
                    Start = sitting.Open.Add(m.Start.TimeOfDay),
                    SittingId = (int)m.SittingId,
                    CustomerNum = m.CustomerNum,
                    Duration = m.Duration,
                    ReservationOriginId = m.ReservationOriginId,
                    Notes = m.Notes,
                    ReservationStatus = (ReservationStatus)m.Status,

                };
                _context.Add(r);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(m);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservations = _context.Reservations
                .Include(r => r.Person)
                .Include(r => r.ReservationOrigin)
                .Include(r => r.Sitting)
                .ThenInclude(s => s.Restaurant)
                .ThenInclude(r => r.Areas)
                .ThenInclude(a => a.Tables);
            var reservation = await reservations.FirstOrDefaultAsync(r => r.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }
            var m = new Models.StaffReservation.Update
            {
                Id = reservation.Id,
                Start = reservation.Start,
                SittingId = reservation.SittingId,
                CustomerNum = reservation.CustomerNum,
                Duration = reservation.Duration,
                ReservationOriginId = reservation.ReservationOriginId,
                Notes = reservation.Notes,
                Status = ((int)reservation.ReservationStatus),
                ReservationOrigins = new SelectList(_context.ReservationOrigins.ToArray(), nameof(ReservationOrigin.Id), nameof(ReservationOrigin.Name)),
                Sittings = new SelectList(_context.Sittings.Where(s => s.IsClosed == false).ToArray(), nameof(Sitting.Id), nameof(Sitting.Open)),
                Areas = new SelectList(_context.Areas.ToArray(), nameof(Area.Id), nameof(Area.Name)),
                Tables = new SelectList(_context.Areas.ToArray(), nameof(Table.Id), nameof(Table.Name))
            };
            return View(m);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Models.StaffReservation.Update m)
        {
            if (id != m.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var r = new Reservation
                {
                    Id = m.Id,
                    Start = m.Start,
                    SittingId = m.SittingId,
                    CustomerNum = m.CustomerNum,
                    Duration = m.Duration,
                    ReservationOriginId = m.ReservationOriginId,
                    Notes = m.Notes,
                    ReservationStatus = (ReservationStatus)m.Status,
                };
                try
                {
                    _context.Update(r);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(r.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(m);
        }

        public IActionResult AddTables(int sittingId, int reservationId)
        {
            var tables = _context.Tables.Include(t => t.Area).ToArray();
            var reservations = _context.Reservations
                .Include(r => r.Tables)
                .Where(r => r.SittingId == sittingId).ToArray();
            List<Table> freeTables = new List<Table>();
            foreach(var r in reservations)
            {
                foreach(var t in tables)
                {
                    if (!r.Tables.Contains(t))
                    {
                        freeTables.Add(t);
                    }
                }
            }
            var reservation = _context.Reservations.FirstOrDefaultAsync(r => r.Id == reservationId);
            var sitting = _context.Sittings.Include(s => s.SittingType).FirstOrDefaultAsync(s => s.Id == sittingId);
            var sResult = sitting.Result;
            var rResult = reservation.Result;
            var m = new Models.StaffReservation.AddTables
            {
                ChosenTables = String.Join(", ", rResult.Tables.Select(t => t.Name)),
                Heads = reservation.Result.CustomerNum,
                SittingInfo = $"{sResult.Open.ToShortDateString()} {sResult.Open.ToShortTimeString()} - {sResult.Close.ToShortTimeString()} {sResult.SittingType.Name}",
                ReservationInfo = $"{rResult.Start.ToShortTimeString()} - {rResult.End.ToShortTimeString()} Status: {rResult.ReservationStatus}",
                SittingId = sittingId,
                ReservationId = reservationId,
                Tables = freeTables,
                Areas = _context.Areas.ToList()
            };

            return View(m);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTables(Models.StaffReservation.AddTables m)
        {
            if (ModelState.IsValid)
            {
                var tables = _context.Tables.ToArray();
                var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == m.ReservationId);
                if(reservation == null)
                {
                    return NotFound();
                }
                foreach(int tableId in m.SelectedTables)
                {
                    reservation.Tables.Add(tables.FirstOrDefault(t => t.Id == tableId));
                }
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                RedirectToAction(nameof(SittingIndex), m.SittingId);
            }
            return View(m);
        }

        public async Task<int> UpdateStatus(string value, string id)
        {
            var reservation = _context.Reservations.FirstOrDefaultAsync(s => s.Id == int.Parse(id));
            reservation.Result.ReservationStatus = (ReservationStatus)int.Parse(value);
            try
            {
                _context.Update(reservation.Result);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception("Attempted to update invalid status");
            }
            return int.Parse(value);
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }
}
