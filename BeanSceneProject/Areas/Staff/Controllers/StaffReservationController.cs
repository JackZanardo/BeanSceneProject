using BeanSceneProject.Areas.Staff.Models.StaffReservation;
using BeanSceneProject.Data;
using BeanSceneProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Areas.Staff.Controllers
{
    [Area("Staff"), Authorize(Roles = "Admin, Staff")]
    public class StaffReservationController : StaffAreaBaseController
    {
        private readonly SittingService _sittingService;
        private readonly PersonService _personService;
        public StaffReservationController(ApplicationDbContext context, SittingService sittingService, PersonService personService) : base(context) 
        {
            _sittingService = sittingService;
            _personService = personService;
        }

        public async Task<IActionResult> SittingIndex()
        {
            var sittings = _context.Sittings
                .Where(s => !s.IsClosed && s.Close >= DateTime.Now)
                .Include(s => s.SittingType)
                .Include(s => s.Restaurant)
                .Include(s => s.Reservations)
                .ThenInclude(r => r.ReservationOrigin)
                .OrderByDescending(s => s.Open).Reverse();
            return View(await sittings.ToListAsync());
        }

        public async Task<IActionResult> Index(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var sitting = await _context.Sittings
                .Include(s => s.SittingType)
                .FirstOrDefaultAsync(s => s.Id == id);
            var reservations = await _context.Reservations
                .Where(r => r.SittingId == id)
                .Include(r => r.Person)
                .Include(r => r.ReservationOrigin)
                .Include(r => r.Tables)
                .OrderByDescending(r => r.Start).Reverse().ToListAsync();
            bool walkInOpen = (sitting.Close >= DateTime.Now && sitting.Open <= DateTime.Now);
            var m = new Models.StaffReservation.Index
            {
                OpenWalkIn = walkInOpen,
                SittingId = id,
                Reservations = reservations,
                SittingInfo = $"{sitting.Open.ToShortDateString()} {sitting.Open.ToShortTimeString()} - {sitting.Close.ToShortTimeString()} {sitting.SittingType.Name}"
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

        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var reservation = await _context.Reservations
                .Include(r => r.Person)
                .Include(r => r.Tables)
                .Include(r => r.Sitting)
                .ThenInclude(s => s.Restaurant)
                .Include(r => r.Sitting)
                .ThenInclude(s => s.SittingType)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }
            return View(reservation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Person)
                .Include(r => r.Tables)
                .Include(r => r.Sitting)
                .ThenInclude(s => s.Restaurant)
                .FirstOrDefaultAsync(r => r.Id == id);
            int sId = reservation.SittingId;
            if (reservation == null)
            {
                return NotFound();
            }
            try
            {
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), sId);
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
        }

        public async Task<IActionResult> Create(int? id)
        {
            if(id == null) { return NotFound(); }
            var sitting = await _context.Sittings
                .Include(s => s.SittingType)
                .Include(s => s.Reservations)
                .FirstOrDefaultAsync(s => s.Id == id);
            var resOrigins = _context.ReservationOrigins.Where(ro => ro.Name != "InPerson" && ro.Name != "Website").ToArray();
            var m = new Create
            {
                SittingId = id,
                ReservationOrigins = new SelectList(resOrigins, nameof(ReservationOrigin.Id), nameof(ReservationOrigin.Name)),
                Available = sitting.Available,
                SittingClose = sitting.Close,
                SittingOpen = sitting.Open,
                SittingInfo = $"{sitting.Open.ToShortDateString()} {sitting.Open.ToShortTimeString()} - {sitting.Close.ToShortTimeString()} {sitting.SittingType.Name}"
            };
            return View(m);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Create m)
        {
            if (ModelState.IsValid)
            {
                var sitting = await _context.Sittings.FirstOrDefaultAsync(s => s.Id == (int)m.SittingId);
                var person = new Person
                {
                    Email = m.Email,
                    FirstName = m.FirstName,
                    LastName = m.LastName,
                    MobileNumber = m.MobileNumber
                };
                person = await _personService.UpsertPersonAsync(person, true);
                if (person == null)
                {
                    ModelState.AddModelError("", "Please provide vaild details");
                    return View(m);
                }
                var r = new Reservation
                {
                    Start = DateTime.Parse(m.Start),
                    SittingId = (int)m.SittingId,
                    CustomerNum = m.CustomerNum,
                    Duration = m.Duration,
                    ReservationOriginId = m.ReservationOriginId,
                    Notes = m.Notes,
                    ReservationStatus = ReservationStatus.Pending,
                    PersonId = person.Id

                };
                _context.Reservations.Add(r);
                await _context.SaveChangesAsync();
                int rId = r.Id;
                return RedirectToAction(nameof(AddTables), "StaffReservation", new { rId = rId, sId = m.SittingId });
            }
            return View(m);
        }

        public async Task<IActionResult> CreateWalkIn(int? id)
        {
            var sitting = await _context.Sittings
                .Include(s => s.SittingType)
                .Include(s => s.Reservations)
                .FirstOrDefaultAsync(s => s.Id == id);
            if(sitting.Close <= DateTime.Now || sitting.Open >= DateTime.Now)
            {
                return RedirectToAction(nameof(Index), "StaffReservation", new { id = id });
            }
            var m = new WalkIn
            {
                SittingId = id,
                Available = sitting.Available,
                SittingClose = sitting.Close,
                SittingOpen = sitting.Open,
                SittingInfo = $"{sitting.Open.ToShortDateString()} {sitting.Open.ToShortTimeString()} - {sitting.Close.ToShortTimeString()} {sitting.SittingType.Name}"
            };
            return View(m);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWalkIn(WalkIn m)
        {
            if (ModelState.IsValid)
            {
                var sitting = await _context.Sittings.FirstOrDefaultAsync(s => s.Id == (int)m.SittingId);
                var resOrigin = await _context.ReservationOrigins.FirstOrDefaultAsync(ro => ro.Name == "InPerson");
                if (sitting == null) { return NotFound(); }
                if (sitting.Close <= DateTime.Now || sitting.Open >= DateTime.Now)
                {
                    return NotFound();
                }
                var person = new Person
                {
                    Email = "WalkIn@Beanscene.com",
                    FirstName = "Walk",
                    LastName = "In",
                    MobileNumber = "0000000000"
                };
                person = await _personService.UpsertPersonAsync(person, true);
                var r = new Reservation
                {
                    SittingId = (int)m.SittingId,
                    Start = DateTime.Now,
                    Duration = m.Duration,
                    ReservationStatus = ReservationStatus.Confirmed,
                    ReservationOriginId = resOrigin.Id,
                    CustomerNum = m.CustomerNum,
                    PersonId = person.Id
                };
                _context.Reservations.Add(r);
                var result = await _context.SaveChangesAsync();
                int rId = r.Id;
                return RedirectToAction(nameof(AddTables), "StaffReservation", new { rId = rId, sId = m.SittingId });
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
            var tables = await _context.Tables.Include(t => t.Area).ToListAsync();
            var reservations = await _context.Reservations
                .Include(r => r.Tables)
                .Where(r => r.SittingId == sId).ToArrayAsync();
            var sittingTables = await _context.Tables
                .Include(r => r.Reservations.Where(r => r.SittingId == sId)).ToListAsync();
            var freeTables = sittingTables.Where(t => t.Reservations.Count == 0);
            ///List<int> freeTables = new List<int>();
            var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == rId);
            var sitting = await _context.Sittings.Include(s => s.SittingType).FirstOrDefaultAsync(s => s.Id == sId);
            var m = new AddTables
            {
                ChosenTables = String.Join(", ", reservation.Tables.Select(t => t.Name)),
                ChosenTablesId = reservation.Tables.Select(t => t.Id).ToArray(),
                Heads = reservation.CustomerNum,
                SittingInfo = $"{sitting.Open.ToShortDateString()} {sitting.Open.ToShortTimeString()} - {sitting.Close.ToShortTimeString()} {sitting.SittingType.Name}",
                ReservationInfo = $"{reservation.Start.ToShortTimeString()} - {reservation.End.ToShortTimeString()} Status: {reservation.ReservationStatus}",
                SittingId = sId,
                ReservationId = rId,
                Tables = await _context.Tables.ToListAsync(),
                Areas = await _context.Areas.ToListAsync(),
                FreeTableIds = freeTables.Select(t => t.Id).ToArray(),
                ReservationNotes = reservation.Notes
            };
            return View(m);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTables(AddTables m)
        {
            if (ModelState.IsValid)
            {
                var tables = _context.Tables.Include(t => t.Reservations);
                var reservation = await _context.Reservations.Include(r => r.Tables).FirstOrDefaultAsync(r => r.Id == m.ReservationId);
                if(reservation == null)
                {
                    return NotFound();
                }
                reservation.Tables.Clear();
                foreach (int tableId in m.SelectedTables)
                {
                    reservation.Tables.Add(tables.FirstOrDefault(t => t.Id == tableId));
                }
                try
                {
                    _context.Tables.UpdateRange(tables);
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
                        throw new Exception("Update failed");
                    }
                }
                return RedirectToAction(nameof(Index), "StaffReservation", new { id = m.SittingId });
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
