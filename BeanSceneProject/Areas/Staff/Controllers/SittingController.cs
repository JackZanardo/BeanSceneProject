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
                .Include(s => s.Reservations);
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
                .FirstOrDefaultAsync(s => s.Id == id);
            if (sitting == null)
            {
                return NotFound();
            }

            return View(sitting);
        }
        
        //GET: Sittings/Create
        public IActionResult Create()
        {
            ViewData["SittingType"] = new SelectList(_context.SittingTypes, "Id");
            ViewData["Area"] = new SelectList(_context.Areas, "Id");
            ViewData["Table"] = new SelectList(_context.Tables, "Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Open,Close,Capacity,RestuarantId,SittingTypeId")] Sitting sitting)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sitting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SittingType"] = new SelectList(_context.SittingTypes, "Id");
            ViewData["Area"] = new SelectList(_context.Areas, "Id");
            ViewData["Table"] = new SelectList(_context.Tables, "Id");
            return View(sitting);

        }
        

        
    }
}
