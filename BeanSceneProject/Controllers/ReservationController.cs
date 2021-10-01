using BeanSceneProject.Data;
using BeanSceneProject.Services;
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
        private readonly PersonService _personService;
        public ReservationController(ApplicationDbContext context, PersonService personService) : base(context) 
        {
            _personService = personService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            var m = new Models.Reservation.Create
            {
                Areas = new SelectList(_context.Areas.ToArray(), nameof(Area.Id), nameof(Area.Name)),
                StartTimes = new SelectList(_context.Sittings.ToArray(), nameof(Sitting.Id), nameof(Sitting.Open))
            };
            return View(m);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.Reservation.Create m)
        {
            if (ModelState.IsValid)
            {
                var person = new Person
                {
                    Email = m.Email,
                    FirstName = m.FirstName,
                    LastName = m.LastName,
                    MobileNumber = m.MobileNumber
                };
                person = await _personService.UpsertPersonAsync(person, false);
                var r = new Reservation
                {
                    Start = m.Start,
                    CustomerNum = m.CustomerNum,
                    Duration = m.Duration,
                    ReservationOriginId = _context.ReservationOrigins.FirstOrDefault(ro => ro.Name == "Website").Id,
                    Notes = m.Notes,
                    ReservationStatus = ReservationStatus.Pending,
                    SittingId = m.SittingId,
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
