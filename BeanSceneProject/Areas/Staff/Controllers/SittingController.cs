using BeanSceneProject.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace BeanSceneProject.Areas.Staff.Controllers
{
    [Area("Staff"), Authorize(Roles = "Admin")]
    public class SittingController : StaffAreaBaseController
    {
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
        

        public IActionResult Create()
        {
            Trace.Listeners.Add(new TextWriterTraceListener("SittingCreateGetOutput.log"));
            Debug.WriteLine("Debugging in SittingController Create GET");
            var m = new Models.Sitting.Create
            {
                StartDate = DateTime.Today,
                Restuarants = new SelectList(_context.Restaurants.ToArray(), nameof(Restaurant.Id), nameof(Restaurant.Name)),
                SittingTypeSelect = new SelectList(_context.SittingTypes.ToArray(), nameof(SittingType.Id), nameof(SittingType.Name)),
                SittingTypes = _context.SittingTypes.ToArray()
            };
            Debug.WriteLine("Checking View model not null");
            Debug.Assert(m is { }, "View model is null");
            Debug.WriteLineIf(m is { }, "View model is not null");
            Debug.WriteLine("Checking View model data");
            Debug.WriteLineIf(m.StartDate != DateTime.Today, "Start date not established");
            Debug.Assert(m.Restuarants is { }, "Restaurant select list is null");
            Debug.Assert(m.SittingTypeSelect is { }, "Sitting type select is null");
            Debug.WriteLine("View model data is correct");
            Trace.Close();
            return View(m);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.Sitting.Create m)
        {
            Trace.Listeners.Add(new TextWriterTraceListener("SittingCreatePostOutput.log"));
            Debug.WriteLine("Debugging in SittingController Create POST");
            if (ModelState.IsValid)
            {
                if (m.StartDate.Add(m.CloseTime.TimeOfDay).Ticks <= DateTime.Now.Ticks)
                {
                    Debug.WriteLine("Model time data invalid");
                    ModelState.AddModelError("", "Sitting ends before current time");
                    Trace.Close();
                    return View(m);
                }
                if (m.StartDate.Add(m.OpenTime.TimeOfDay).Ticks <= DateTime.Now.Ticks)
                {
                    Debug.WriteLine("Model time data invalid");
                    ModelState.AddModelError("", "Sitting begins before current time");
                    Trace.Close();
                    return View(m);
                }
                if (m.StartDate.Add(m.OpenTime.TimeOfDay).Ticks >= m.StartDate.Add(m.CloseTime.TimeOfDay).Ticks)
                {
                    Debug.WriteLine("Model time data invalid");
                    ModelState.AddModelError("", "Sitting begins before close time");
                    Trace.Close();
                    return View(m);
                }
                if (m.Capacity < 0)
                {
                    Debug.WriteLine("Model Capacity invalid");
                    ModelState.AddModelError("", "Can not enter a negative capacity");
                    Trace.Close();
                    return View(m);
                }
                if (!SittingTypeExists(m.SittingTypeId))
                {
                    Debug.WriteLine("Model sitting type invalid");
                    ModelState.AddModelError("", "Invalid or no sitting type selected");
                    Trace.Close();
                    return View(m);
                }
                if (!RestaurantExists(m.RestuarantId))
                {
                    Debug.WriteLine("Model Restaurant invalid");
                    ModelState.AddModelError("", "Invalid or no restuarant selected");
                    Trace.Close();
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
                Debug.Assert(s is { }, "Failed to create instance of Sitting");
                Debug.WriteLineIf(s is { }, "Instance of sitting created");
                try
                {
                    _context.Add(s);
                    var result = await _context.SaveChangesAsync();
                    Debug.Assert(result == 1, "Failed to persist sitting");
                    Debug.WriteLineIf(result == 1, "1 row entered into Sittings table");
                    Debug.WriteLine("Sitting successfully persisted");
                    Trace.Close();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    Debug.WriteLine("DbUpdateException occurred. Failed to add new Sitting.");
                    throw new Exception("Failed to add Sitting to database");
                }
            }
            Debug.WriteLine("Model State is invalid");
            Trace.Close();
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
                Id = sitting.Id,
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
        public async Task<IActionResult> Edit(int? id, Models.Sitting.Update m)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var s = await _context.Sittings.FirstOrDefaultAsync(s => s.Id == id);
                if(s == null)
                {
                    return NotFound();
                }
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
                        throw new Exception("Sitting update failed");
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

        public async Task<bool> UpdateIsClosed(string value, string id)
        {
            var sitting = _context.Sittings.FirstOrDefaultAsync(s => s.Id == int.Parse(id));
            sitting.Result.IsClosed = bool.Parse(value);
            try
            {
                _context.Update(sitting.Result);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
            return true;
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
