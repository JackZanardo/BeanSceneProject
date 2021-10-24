using BeanSceneProject.Areas.Staff.Controllers;
using BeanSceneProject.Controllers;
using BeanSceneProject.Data;
using BeanSceneProject.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    class SittingControllerTest
    {
        private ApplicationDbContext _context;
        private SittingController _controller;

        [ClassInitialize]
        public void TestFixtureSetup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server = localhost; Database = BeanSceneDatabase; Trusted_Connection = True; MultipleActiveResultSets = true")
                .Options;
            _context = new ApplicationDbContext(options);

            _controller = new SittingController(_context);
        }

        [TestMethod]
        public void IndexTestNotNull()
        {

            var result = _controller.Index();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CreateSittingTestNotNull()
        {
            var model = new BeanSceneProject.Areas.Staff.Models.Sitting.Create
            {
                OpenTime = DateTime.Today.AddHours(8),
                CloseTime = DateTime.Today.AddHours(11),
                RestuarantId = _context.Restaurants.First().Id,
                SittingTypeId = _context.SittingTypes.FirstOrDefault(st => st.Name == "Lunch").Id,
                Capacity = 40
            };
            var result = _controller.Create(model);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        }

        [TestMethod]
        public void CreateSittingTestDatabase()
        {
            var model = new BeanSceneProject.Areas.Staff.Models.Sitting.Create
            {
                OpenTime = DateTime.Today.AddHours(8),
                CloseTime = DateTime.Today.AddHours(11),
                RestuarantId = _context.Restaurants.First().Id,
                SittingTypeId = _context.SittingTypes.FirstOrDefault(st => st.Name == "Lunch").Id,
                Capacity = 40
            };
            var result = _controller.Create(model);

            var sitting = _context.Sittings.FirstOrDefault(s => s.Open == model.OpenTime);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            Assert.IsNotNull(sitting);
        }

        [TestMethod]
        public void EditSittingTestDatabase()
        {
            var sitting = _context.Sittings.First();

            var model = new BeanSceneProject.Areas.Staff.Models.Sitting.Update
            {
                Id = sitting.Id,
                Open = sitting.Open,
                Close = sitting.Close,
                RestuarantId = sitting.RestaurantId,
                SittingTypeId = sitting.SittingTypeId,
                Capacity = 42,
                IsClosed = true
            };

            var viewResult = _controller.Edit(sitting.Id, model);
            var sittingResullt = _context.Sittings.FirstOrDefault(s => s.Id == sitting.Id);

            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(sittingResullt);
            Assert.AreEqual(42, sittingResullt.Capacity);
            Assert.IsTrue(sittingResullt.IsClosed);

        }


    }
}
