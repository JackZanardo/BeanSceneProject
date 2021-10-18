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
        private readonly SittingService _sittingService;
        public ReservationController(ApplicationDbContext context, PersonService personService, SittingService sittingService) : base(context) 
        {
            _personService = personService;
            _sittingService = sittingService;
        }

        public IActionResult Index()
        {
            return View();
        }

        
        public IActionResult Create()
        {
            var m = new Create
            {
                StartDates = JsonSerializer.Serialize(_context.Sittings.Select(s => s.Open.Date).ToList())
            };
            return View(m);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Create m)
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
                    Start = DateTime.Parse(m.StartTime),
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
            var sittings = _sittingService.GetSittingsAsync(date, false);
            var sittingsResult = sittings.Result;
            var sessions = new List<Session>();
            foreach(var s in sittingsResult)
            {
                sessions.Add(new Session
                {
                    Id = s.Id,
                    InfoText = $"{s.Open.ToShortTimeString()} - {s.Close.ToShortTimeString()} {s.SittingType.Name}",
                    SittingOpen = s.Open,
                    SittingClose = s.Close
                });
            }
            var jsonSessions = JsonSerializer.Serialize(sessions);
            
            return new JsonResult(jsonSessions);
        }

        //This may not be necessary. Has been replaced with client side javascript
        public JsonResult GetStartTimes(string sittingId)
        {
            var sitting =  _sittingService.GetSittingAsync(Int32.Parse(sittingId));
            var s = sitting.Result;
            var times = new List<Session>();
            DateTime interval = s.Open;
            while(interval < s.Close)
            {
                times.Add(new Session
                { 
                    InfoText = interval.ToShortTimeString(),
                    Start = interval
                });
                interval =  interval.AddMinutes(15);
            }

            var jsonTimes = JsonSerializer.Serialize(times);

            return new JsonResult(jsonTimes);
        }

    }
}
