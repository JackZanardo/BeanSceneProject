using BeanSceneProject.Data;
using BeanSceneProject.Models.Reservation;
using BeanSceneProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
                StartDates = JsonSerializer.Serialize(_context.Sittings.Select(s => s.Open.Date).ToList())
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
                    CustomerNum = m.CustomerNum,
                    Duration = m.Duration,
                    ReservationOriginId = _context.ReservationOrigins.FirstOrDefault(ro => ro.Name == "Website").Id,
                    Notes = m.Notes,
                    ReservationStatus = ReservationStatus.Pending,
                    PersonId = person.Id
                };
                _context.Add(r);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(m);
        }

        public JsonResult GetSittings(string jsonDate)
        {
            var date = DateTime.Parse(jsonDate).Date;
            var sittings = _context.Sittings
                .Include(s => s.SittingType)
                .Where(s => s.Open.Date == date && s.IsClosed == false);
            var sessions = new List<Session>();
            foreach(var s in sittings)
            {
                sessions.Add(new Session
                {
                    Id = s.Id,
                    InfoText = $"{s.Open.ToLongTimeString()} {s.SittingType.Name}"
                });
            }
            var jsonSessions = JsonSerializer.Serialize(sessions);
            
            return new JsonResult(jsonSessions);
        }
    }
}
