using BeanSceneProject.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Controllers
{
    public class PersonController : BaseController
    {
        public PersonController(ApplicationDbContext context) : base(context) {}
        public IActionResult Index()
        {
            return View();
        }
    }
}
