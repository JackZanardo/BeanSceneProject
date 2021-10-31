using BeanSceneProject.Data;
using BeanSceneProject.Models.Reservation;
using BeanSceneProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace BeanSceneProject.Controllers
{
    public class ReservationController : BaseController
    {
        private readonly PersonService _personService;
        private readonly SittingService _sittingService;
        private readonly UserManager<IdentityUser> _userManager;
        public ReservationController(ApplicationDbContext context, PersonService personService, SittingService sittingService, UserManager<IdentityUser> userManager) : base(context) 
        {
            _userManager = userManager;
            _personService = personService;
            _sittingService = sittingService;
        }

        public IActionResult Index()
        {
            return View();
        }

        
        public async Task<IActionResult> Create()
        {
            var sittings = await _context.Sittings.Where(s => !s.IsClosed).Include(s => s.Reservations).ToListAsync();
            var openSittings = sittings.Where(s => s.Close.Ticks >= DateTime.Now.Ticks);
            var avaliable = sittings.Where(s => s.Available > 0).Select(s => s.Open.Date);
            var m = new Create
            {   
                StartDates = JsonSerializer.Serialize(avaliable)
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
                person = await _personService.UpsertPersonAsync(person, true);
                if(person == null)
                {
                    ModelState.AddModelError("", "Please provide vaild details");
                    return View(m);
                }
                if (IsDoubleBooked(m.SittingId, person.Id).Result)
                {
                    ModelState.AddModelError("", "You have already made a booking for this session");
                    return View(m);
                }
                if (!IsSittingValid(m.SittingId).Result)
                {
                    ModelState.AddModelError("", "Please do not try to hack");
                    return View(m);
                }
                if (!IsSittingValid(m.SittingId, m.CustomerNum).Result)
                {
                    ModelState.AddModelError("", "Not enough seats available");
                    return View(m);
                }
                if (!IsReservationTimeValid(m.SittingId, DateTime.Parse(m.StartTime), m.Duration).Result)
                {
                    ModelState.AddModelError("", "Invalid reservation times");
                    return View(m);
                }
                var r = new Reservation
                {
                    SittingId = m.SittingId,
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
                return RedirectToAction(nameof(Index), "Home");
            }
            return View(m);
        }

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> MemberCreate()
        {
            ClaimsPrincipal currentUser = this.User;
            var user = _userManager.GetUserAsync(currentUser);
            var person = _personService.GetPersonAsync(user.Result.Id);
            var p = person.Result;
            var sittings = await _context.Sittings.Where(s => !s.IsClosed).Include(s => s.Reservations).ToListAsync();
            var openSittings = sittings.Where(s => s.Close.Ticks >= DateTime.Now.Ticks);
            var avaliable = sittings.Where(s => s.Available > 0).Select(s => s.Open.Date);
            var m = new Create
            {
                Email = p.Email,
                FirstName = p.FirstName,
                LastName = p.LastName,
                MobileNumber = p.MobileNumber,
                StartDates = JsonSerializer.Serialize(avaliable)
            };
            return View(m);
        }

        [Authorize(Roles = "Member")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MemberCreate(Create m)
        {
            if (ModelState.IsValid)
            {
                ClaimsPrincipal currentUser = this.User;
                var user = _userManager.GetUserAsync(currentUser);
                var person = _personService.GetPersonAsync(user.Result.Id);
                var p = person.Result;
                if (IsDoubleBooked(m.SittingId, p.Id).Result)
                {
                    ModelState.AddModelError("", "You have already made a booking for this session.");
                    return View(m);
                }
                if (!IsSittingValid(m.SittingId).Result)
                {
                    ModelState.AddModelError("", "Please do not try to hack");
                    return View(m);
                }
                if (!IsSittingValid(m.SittingId, m.CustomerNum).Result)
                {
                    ModelState.AddModelError("", "Not enough seats available");
                    return View(m);
                }
                if (!IsReservationTimeValid(m.SittingId, DateTime.Parse(m.StartTime), m.Duration).Result)
                {
                    ModelState.AddModelError("", "Invalid reservation times");
                    return View(m);
                }
                var r = new Reservation
                {
                    SittingId = m.SittingId,
                    Start = DateTime.Parse(m.StartTime),
                    CustomerNum = m.CustomerNum,
                    Duration = m.Duration,
                    ReservationOriginId = _context.ReservationOrigins.FirstOrDefault(ro => ro.Name == "Website").Id,
                    Notes = m.Notes,
                    ReservationStatus = ReservationStatus.Pending,
                    PersonId = p.Id
                };
                _context.Add(r);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), "Home");
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
                    SittingClose = s.Close,
                    Available = s.Available
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

        public async Task<bool> IsDoubleBooked(int sittingId, int personId)
        {
            var sitting = await _sittingService.GetSittingAsync(sittingId);
            var reservations = await _context.Reservations.Where(r => r.SittingId == sitting.Id).ToListAsync();
            var doubleBook = reservations.FirstOrDefault(r => r.PersonId == personId);
            if(doubleBook == null)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> IsSittingValid(int sittingId)
        {
            var sitting = await _sittingService.GetSittingAsync(sittingId);
            if (sitting.IsClosed)
            {
                return false;
            }
            if (sitting.Close.Ticks <= DateTime.Now.Ticks)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> IsSittingValid(int sittingId, int customerNum)
        {
            var sitting = await _sittingService.GetSittingAsync(sittingId);
            if (sitting.Available >= customerNum)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> IsReservationTimeValid(int sittingId, DateTime start, int duration)
        {
            var sitting = await _sittingService.GetSittingAsync(sittingId);
            if (sitting.Open > start) { return false; }
            if (sitting.Close < start.AddMinutes(duration)) { return false; }
            return true;
        }
    }
}
