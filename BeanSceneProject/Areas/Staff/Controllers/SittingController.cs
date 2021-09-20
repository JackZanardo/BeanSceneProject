using BeanSceneProject.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Areas.Staff.Controllers
{
    [Area("Staff"), Authorize(Roles = "Admin")]
    public class SittingController : StaffAreaBaseController
    {
        public SittingController(ApplicationDbContext context) : base(context) { }
        
    }
}
