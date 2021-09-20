using BeanSceneProject.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Areas.Staff.Controllers
{
    public abstract class StaffAreaBaseController : Controller
    {
        protected readonly ApplicationDbContext _context;
        public StaffAreaBaseController(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
