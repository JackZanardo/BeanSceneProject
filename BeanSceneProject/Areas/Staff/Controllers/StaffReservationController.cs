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
                return RedirectToAction(nameof(Index), "StaffReservation", new { id = m.SittingId });
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
        public async Task<IActionResult> Edit(int? id, Models.StaffReservation.Update m)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == id);
                if(reservation == null)
                {
                    return NotFound();
                }
                reservation.SittingId = m.SittingId;
                reservation.Start = m.Start;
                reservation.Duration = m.Duration;
                reservation.CustomerNum = m.CustomerNum;
                reservation.Notes = m.Notes;
                reservation.ReservationOriginId = m.ReservationOriginId;
                reservation.ReservationStatus = (ReservationStatus)m.Status;
                reservation.PersonId = m.PersonId;
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
                        throw new Exception("Reservation update failed");
                    }
                }
                return RedirectToAction(nameof(Index), m.SittingId);
            }

            return View(m);
        }

        public async Task<IActionResult> AddTables(int rId, int sId)
        {
            var tables = _context.Tables.Include(t => t.Area).ToArray();
            var reservations = await _context.Reservations
                .Include(r => r.Tables)
                .Where(r => r.SittingId == sId).ToArrayAsync();
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
            var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == rId);
            var sitting = await _context.Sittings.Include(s => s.SittingType).FirstOrDefaultAsync(s => s.Id == sId);
            var m = new Models.StaffReservation.AddTables
            {
                ChosenTables = String.Join(", ", reservation.Tables.Select(t => t.Name)),
                Heads = reservation.CustomerNum,
                SittingInfo = $"{sitting.Open.ToShortDateString()} {sitting.Open.ToShortTimeString()} - {sitting.Close.ToShortTimeString()} {sitting.SittingType.Name}",
                ReservationInfo = $"{reservation.Start.ToShortTimeString()} - {reservation.End.ToShortTimeString()} Status: {reservation.ReservationStatus}",
                SittingId = sId,
                ReservationId = rId,
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
                if(m.SelectedTables.Count() == 0)
                {
                    return RedirectToAction(nameof(Index), "StaffReservation", new { id = m.SittingId });
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
                return RedirectToAction(nameof(Index), "StaffReservation", new { id = m.SittingId});
            }
            return View(m);
        }

        public async Task<bool> UpdateStatus(string value, string id)
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
                return false;
            }
            return true;
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }
}
