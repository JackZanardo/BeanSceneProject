using Microsoft.VisualStudio.TestTools.UnitTesting;
using BeanSceneProject.Areas.Staff.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeanSceneProject.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace BeanSceneProjectTests
{
    [TestClass]
    public class SittingControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly SittingController _controller;

        public SittingControllerTests()
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
            var viewResult = _controller.Create(model);
            viewResult.Wait();
            var result = viewResult.Result;

            var sitting = _context.Sittings.FirstOrDefaultAsync(s => s.Open == model.OpenTime);

            Assert.IsNotNull(result);
            Assert.IsNotNull(sitting);
        }

        [TestMethod]
        public void EditSittingTestDatabase()
        {
            var sitting = _context.Sittings.First();

            var model = new BeanSceneProject.Areas.Staff.Models.Sitting.Update
            {
                Sitting = sitting,
                Open = sitting.Open,
                Close = sitting.Close,
                RestuarantId = sitting.RestaurantId,
                SittingTypeId = sitting.SittingTypeId,
                Capacity = 42,
                IsClosed = true
            };

            var viewResult = _controller.Edit(model.Sitting.Id, model);
            viewResult.Wait();
            
            var sittingResullt = _context.Sittings.FirstOrDefaultAsync(s => s.Id == model.Sitting.Id);
            sittingResullt.Wait();

            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(sittingResullt);
            Assert.AreEqual(42, sittingResullt.Result.Capacity);
            Assert.IsTrue(sittingResullt.Result.IsClosed);

        }
    }
}
