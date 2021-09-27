using BeanSceneProject.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Areas.Staff.Controllers
{
    [Area("Staff"), Authorize(Roles = "Staff,Admin")]
    public class HomeController : StaffAreaBaseController
    {
        public HomeController(ApplicationDbContext context) : base(context) {}

        public IActionResult Index()
        {
            return View();
        }
    }
}
