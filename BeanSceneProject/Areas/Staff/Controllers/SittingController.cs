using BeanSceneProject.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace BeanSceneProject.Areas.Staff.Controllers
{
    [Area("Staff"), Authorize(Roles = "Admin")]
    public class SittingController : StaffAreaBaseController
    {
        private static Logger logger = LogManager.GetLogger("LoggerRules");
        public SittingController(ApplicationDbContext context) : base(context) { }

        //GET: Sittings with sitting type and reservations
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Sittings
                .Include(s => s.SittingType)
                .Include(s => s.Reservations)
                .Include(s => s.Restaurant);
            return View(await applicationDbContext.ToListAsync());
        }

        //GET: Sittings/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sitting = await _context.Sittings
                .Include(s => s.SittingType)
                .Include(s => s.Reservations)
                .ThenInclude(r => r.Tables)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (sitting == null)
            {
                return NotFound();
            }
            var model = new Models.Sitting.Details
            {
                Id = sitting.Id,
                Open = sitting.Open,
                Close = sitting.Close,
                IsClosed = sitting.IsClosed,
                Capacity = sitting.Capacity,
                Heads = sitting.Heads,
                SittingType = sitting.SittingType.Name,
                Reservations = sitting.Reservations.Count(),
                BookedTables = String.Join(", ",sitting.Reservations.Select(r => r.Tables.Select(t => t.Name)))
            };
            return View(model);
        }
        
        //Allow anonymous added for testing
        //Remove when finished
        [AllowAnonymous]
        public IActionResult Create()
        {
            var m = new Models.Sitting.Create
            {
                StartDate = DateTime.Today,
                Restraunts = new SelectList(_context.Restaurants.ToArray(), nameof(Restaurant.Id), nameof(Restaurant.Name)),
                SittingTypeSelect = new SelectList(_context.SittingTypes.ToArray(), nameof(SittingType.Id), nameof(SittingType.Name)),
                SittingTypes = _context.SittingTypes.ToArray()
            };
            return View(m);
        }

        //Allow anonymous added for testing
        //Remove when finished
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Create(Models.Sitting.Create m)
        {
            logger.Info("Entering Create Method In Sitting Controller");
            if (ModelState.IsValid)
            {
                if (m.StartDate.Add(m.CloseTime.TimeOfDay).Ticks <= DateTime.Now.Ticks)
                {
                    ModelState.AddModelError("", "Sitting ends before current time");
                    logger.Error("Exiting Create Method In Sitting Controller. Sitting Creation Failed Due To Incorrect Data");
                    return View(m);
                }
                if (m.StartDate.Add(m.OpenTime.TimeOfDay).Ticks <= DateTime.Now.Ticks)
                {
                    ModelState.AddModelError("", "Sitting begins before current time");
                    logger.Error("Exiting Create Method In Sitting Controller. Sitting Creation Failed Due To Incorrect Data");
                    return View(m);
                }
                if (m.StartDate.Add(m.OpenTime.TimeOfDay).Ticks >= m.StartDate.Add(m.CloseTime.TimeOfDay).Ticks)
                {
                    ModelState.AddModelError("", "Sitting begins before close time");
                    logger.Error("Exiting Create Method In Sitting Controller. Sitting Creation Failed Due To Incorrect Data");
                    return View(m);
                }
                if (m.Capacity <= 0)
                {
                    ModelState.AddModelError("", "Please enter a positive whole number for capacity");
                    logger.Error("Exiting Create Method In Sitting Controller. Sitting Creation Failed Due To Incorrect Data");
                    return View(m);
                }
                if (!SittingTypeExists(m.SittingTypeId))
                {
                    ModelState.AddModelError("", "Invalid or no sitting type selected");
                    logger.Error("Exiting Create Method In Sitting Controller. Sitting Creation Failed Due To Incorrect Data");
                    return View(m);
                }
                if (!RestaurantExists(m.RestuarantId))
                {
                    ModelState.AddModelError("", "Invalid or no restuarant selected");
                    logger.Error("Exiting Create Method In Sitting Controller. Sitting Creation Failed Due To Incorrect Data");
                    return View(m);
                }
                var s = new Sitting
                {
                    Open = m.StartDate.Add(m.OpenTime.TimeOfDay),
                    Close = m.StartDate.Add(m.CloseTime.TimeOfDay),
                    Capacity = m.Capacity,
                    SittingTypeId = m.SittingTypeId,
                    RestaurantId = m.RestuarantId,
                    IsClosed = false
                };
                _context.Add(s);
                await _context.SaveChangesAsync();
                logger.Info("Exiting Create Method In Sitting Controller. Sitting Creation Successful. Sitting Successfully Stored in Database");
                return RedirectToAction(nameof(Index));
            }
            return View(m);

        }

        //GET: Sittings/Edit/
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var sitting = await _context.Sittings.FirstOrDefaultAsync(s => s.Id == id);
            if (sitting == null)
            {
                return NotFound();
            }
            var m = new Models.Sitting.Update
            {
                Sitting = sitting,
                Open = sitting.Open,
                Close = sitting.Close,
                Capacity = sitting.Capacity,
                SittingTypeId = sitting.SittingTypeId,
                SittingTypes = new SelectList(_context.SittingTypes.ToArray(), nameof(SittingType.Id), nameof(SittingType.Name)),
                RestuarantId = sitting.RestaurantId,
                Restraunts = new SelectList(_context.Restaurants.ToArray(), nameof(Restaurant.Id), nameof(Restaurant.Name)),
                IsClosed = sitting.IsClosed,
                IsClosedSelect = new SelectList(new Dictionary<bool, string>()
                {
                    {false, "Open"},
                    {true, "Closed"}
                }, "Key", "Value")
            };
            return View(m);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Models.Sitting.Update m)
        {
            if (id != m.Sitting.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var s = m.Sitting;
                s.Open = m.Open;
                s.Close = m.Close;
                s.Capacity = m.Capacity;
                s.RestaurantId = m.RestuarantId;
                s.SittingTypeId = m.SittingTypeId;
                s.IsClosed = m.IsClosed;
                try
                {
                    _context.Update(s);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SittingExists(s.Id))
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

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var sitting = await _context.Sittings
                .Include(s => s.SittingType)
                .Include(s => s.Reservations)
                .ThenInclude(r => r.Tables)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (sitting == null)
            {
                return NotFound();
            }
            var m = new Models.Sitting.Delete
            {
                Id = sitting.Id,
                Open = sitting.Open,
                Close = sitting.Close,
                IsClosed = sitting.IsClosed,
                Capacity = sitting.Capacity,
                Heads = sitting.Heads,
                SittingType = sitting.SittingType.Name,
                Reservations = sitting.Reservations.Count()
            };
            foreach (var r in sitting.Reservations)
            {
                foreach (var t in r.Tables)
                {
                    m.BookedTables.Add(t.Name);
                }
            }
            return View(m);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, Models.Sitting.Delete m)
        {
            if (ModelState.IsValid)
            {
                Sitting s = _context.Sittings.Find(id);
                try
                {
                    _context.Sittings.Remove(s);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SittingExists(s.Id))
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

        public async Task<IActionResult> Report(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sitting = await _context.Sittings
                .Include(s => s.Reservations)
                .ThenInclude(r => r.Tables)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (sitting == null)
            {
                return NotFound();
            }
            var model = new Models.Sitting.Report
            {
                Id = sitting.Id,
                Heads = sitting.Heads,
                BookedTables = String.Join(", ", sitting.Reservations.Select(r => r.Tables.Select(t => t.Name))),
                TotalReservations = sitting.Reservations.Count(),
                PendingReservations = sitting.Reservations.Count(s => s.ReservationStatus == ReservationStatus.Pending),
                ConfirmedReservations = sitting.Reservations.Count(s => s.ReservationStatus == ReservationStatus.Confirmed),
                CompletedReservations = sitting.Reservations.Count(s => s.ReservationStatus == ReservationStatus.Complete),
                CancelledReservations = sitting.Reservations.Count(s => s.ReservationStatus == ReservationStatus.Cancelled),
            };
            return View(model);
        }

        private bool SittingExists(int id)
        {
            return _context.Sittings.Any(e => e.Id == id);
        }

        private bool SittingTypeExists(int id)
        {
            return _context.SittingTypes.Any(st => st.Id == id);
        }

        private bool RestaurantExists(int id)
        {
            return _context.Restaurants.Any(r => r.Id == id);
        }

    }
}
