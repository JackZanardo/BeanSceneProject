using BeanSceneProject.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly ApplicationDbContext _context;
        public BaseController(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
