using BeanSceneProject.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Areas.Member.Controllers
{
    public abstract class MemberAreaBaseController : Controller
    {
        protected readonly ApplicationDbContext _context;
        public MemberAreaBaseController(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
