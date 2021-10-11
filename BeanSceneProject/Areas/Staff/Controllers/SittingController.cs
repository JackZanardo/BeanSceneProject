using BeanSceneProject.Data;
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
                Reservations = sitting.Reservations.Count()
            };
            foreach(var r in sitting.Reservations)
            {
                foreach(var t in r.Tables)
                {
                    model.BookedTables.Add(t.Name);
                }
            }
            return View(model);
        }
        
        //GET: Sittings/Create
        public IActionResult Create()
        {
            var m = new Models.Sitting.Create
            {
                Open = DateTime.Today.AddHours(8),
                Close = DateTime.Today.AddHours(11),
                Restraunts = new SelectList(_context.Restaurants.ToArray(), nameof(Restaurant.Id), nameof(Restaurant.Name)),
                SittingTypes = new SelectList(_context.SittingTypes.ToArray(), nameof(SittingType.Id), nameof(SittingType.Name))
            };
            return View(m);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.Sitting.Create m)
        {
            if (ModelState.IsValid)
            {
                var s = new Sitting
                {
                    Open = m.Open,
                    Close = m.Close,
                    Capacity = m.Capacity,
                    SittingTypeId = m.SittingTypeId,
                    RestuarantId = m.RestuarantId
                };
                _context.Add(s);
                await _context.SaveChangesAsync();
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
                Id = sitting.Id,
                Open = sitting.Open,
                Close = sitting.Close,
                Capacity = sitting.Capacity,
                SittingTypeId = sitting.SittingTypeId,
                SittingTypes = new SelectList(_context.SittingTypes.ToArray(), nameof(SittingType.Id), nameof(SittingType.Name)),
                RestuarantId = sitting.RestuarantId,
                Restraunts = new SelectList(_context.Restaurants.ToArray(), nameof(Restaurant.Id), nameof(Restaurant.Name))
            };
            return View(m);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Models.Sitting.Update m)
        {
            if (id != m.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var s = new Sitting
                {
                    Id = m.Id,
                    Open = m.Open,
                    Close = m.Close,
                    Capacity = m.Capacity,
                    SittingTypeId = m.SittingTypeId,
                    RestuarantId = m.RestuarantId
                };
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


        private bool SittingExists(int id)
        {
            return _context.Sittings.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Delete(int? id)
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
            return RedirectToAction(nameof(Index));
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var sitting = await _context.Sittings.FirstOrDefaultAsync(s => s.Id == id);
            Sitting s = _context.Sittings.Find(sitting);
            _context.Sittings.Remove(s);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
