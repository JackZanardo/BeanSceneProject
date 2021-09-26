using BeanSceneProject.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Controllers
{
    public class ReservationController : BaseController
    {
        public ReservationController(ApplicationDbContext context) : base(context) {}

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            var m = new Models.Reservation.Create
            {
                Areas = new SelectList(_context.Areas.ToArray(), nameof(Area.Id), nameof(Area.Name)),

            };
            return View(m);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.Reservation.Create m)
        {
            if (ModelState.IsValid)
            {
                int sittingId = _context.Sittings.FirstOrDefault(s => (s.Open <= m.Start && s.Close >= (m.Start))).Id;
                var personController = new PersonController(_context);
                var person = personController.FindOrCreatePerson(m.Email, m.FirstName, m.LastName, m.MobileNumber);
                var r = new Reservation
                {
                    Start = m.Start,
                    CustomerNum = m.CustomerNum,
                    Duration = m.Duration,
                    ReservationOriginId = _context.ReservationOrigins.FirstOrDefault(ro => ro.Name == "Website").Id,
                    Notes = m.Notes,
                    ReservationStatus = ReservationStatus.Pending,
                    SittingId = sittingId,
                    PersonId = person.Id
                };
                _context.Add(r);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(m);
        }
    }
}
