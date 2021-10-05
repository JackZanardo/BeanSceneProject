using BeanSceneProject.Controllers;
using BeanSceneProject.Data;
using BeanSceneProject.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsUnitTest
{
    [TestClass]
    class ReservationControllerTest
    {
        [TestMethod]
        public void RendersIndexView()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("BeanSceneDatabase")
                .Options;
            var context = new ApplicationDbContext(options);
            var userManager = new UserManager();
            var personService = new PersonService(context, );
        }
    }
}
