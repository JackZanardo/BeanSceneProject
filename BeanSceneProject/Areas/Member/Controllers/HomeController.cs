using BeanSceneProject.Areas.Member.Controllers;
using BeanSceneProject.Areas.Member.Models;
using BeanSceneProject.Data;
using BeanSceneProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BeanSceneProject.Areas.Member.Controllers
{
    [Area("Member"), Authorize(Roles = "Member")]
    public class HomeController : MemberAreaBaseController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly PersonService _personService;

        private readonly Expression<Func<Reservation, bool>> UpComingReservations =
            r => r.ReservationStatus == ReservationStatus.Pending || r.ReservationStatus == ReservationStatus.Confirmed;

        public HomeController(ApplicationDbContext context, UserManager<IdentityUser> userManager, PersonService personService) : base(context) 
        {
            _personService = personService;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult History()
        {
            ClaimsPrincipal currentUser = this.User;
            var user = _userManager.GetUserAsync(currentUser);
            var person = _personService.GetPersonAsync(user.Result.Id);
            var p = person.Result;
            var m = new History
            {
                Person = p,
                Reservations = _context.Reservations
                    .Include(r => r.Sitting)
                    .ThenInclude(s => s.Restaurant)
                    .Include(r => r.Sitting)
                    .ThenInclude(s => s.SittingType)
                    .Where(r => r.PersonId == p.Id && r.ReservationStatus == ReservationStatus.Complete)
                    .OrderByDescending(r => r.Start)
                    .Reverse()
                    .ToList()
            };

            return View(m);
        }

        public IActionResult UpComing()
        {
            ClaimsPrincipal currentUser = this.User;
            var user = _userManager.GetUserAsync(currentUser);
            var person = _personService.GetPersonAsync(user.Result.Id);
            var p = person.Result;
            var m = new History
            {
                Person = p,
                Reservations = _context.Reservations
                    .Include(r => r.Sitting)
                    .ThenInclude(s => s.Restaurant)
                    .Include(r => r.Sitting)
                    .ThenInclude(s => s.SittingType)
                    .Where(r => r.PersonId == p.Id)
                    .Where(UpComingReservations)
                    .OrderByDescending(r => r.Start)
                    .Reverse()
                    .ToList()
            };

            return View(m);
        }
    }
}
