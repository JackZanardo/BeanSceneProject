﻿using BeanSceneProject.Data;
using BeanSceneProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Areas.Staff.Controllers
{
    public class StaffReservationController : StaffAreaBaseController
    {
        private readonly SittingService _sittingService;
        public StaffReservationController(ApplicationDbContext context, SittingService sittingService) : base(context) 
        {
            _sittingService = sittingService;
        }
        public async Task<IActionResult> Index(int? sittingId)
        {
            if(sittingId == null)
            {
                return NotFound();
            }
            var reservations = _context.Reservations
                .Where(r => r.SittingId == sittingId)
                .Include(r => r.Person)
                .Include(r => r.ReservationOrigin)
                .Include(r => r.Sitting)
                .ThenInclude(s => s.Restaurant)
                .ThenInclude(r => r.Areas)
                .ThenInclude(a => a.Tables);
            var m = new Models.StaffReservation.Index
            {
                SittingId = sittingId,
                Reservations = await reservations.ToListAsync()
            };
            if (reservations == null)
            {
                return NotFound();
            }
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

        public IActionResult Create(int? sittingId)
        {
            var m = new Models.StaffReservation.Create
            {
                SittingId = sittingId,
                Start = _sittingService.GetSittingAsync((int)sittingId).Result.Open,
                ReservationOrigins = new SelectList(_context.ReservationOrigins.ToArray(), nameof(ReservationOrigin.Id), nameof(ReservationOrigin.Name)),
                Areas = new SelectList(_context.Areas.ToArray(), nameof(Area.Id), nameof(Area.Name)),
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
                var r = new Reservation
                {
                    Start = m.Start,
                    SittingId = (int)m.SittingId,
                    CustomerNum = m.CustomerNum,
                    Duration = m.Duration,
                    ReservationOriginId = m.ReservationOriginId,
                    Notes = m.Notes,
                    ReservationStatus = ReservationStatus.Pending,

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

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }
}