using Microsoft.VisualStudio.TestTools.UnitTesting;
using BeanSceneProject.Areas.Staff.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                .UseSqlServer(@"Server = localhost\sqlexpress; Database = BeanSceneDatabase; Trusted_Connection = True; MultipleActiveResultSets = true")
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
        public void CreateSittingTestDatabase()
        {
            var cs = _context.Database.GetConnectionString();
            var r = _context.Restaurants.FirstOrDefault();
            var model = new BeanSceneProject.Areas.Staff.Models.Sitting.Create
            {
                StartDate = DateTime.Today.AddHours(24),
                OpenTime = DateTime.Today.AddHours(32),
                CloseTime = DateTime.Today.AddHours(35),
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
        public void CreateSittingTestDatabase_InvalidDate()
        {
            var cs = _context.Database.GetConnectionString();
            var r = _context.Restaurants.FirstOrDefault();
            var originalcount = _context.Sittings.Count();
            var model = new BeanSceneProject.Areas.Staff.Models.Sitting.Create
            {
                OpenTime = new DateTime(2015, 12, 31),
                CloseTime = DateTime.Today.AddHours(35),
                RestuarantId = _context.Restaurants.First().Id,
                SittingTypeId = _context.SittingTypes.FirstOrDefault(st => st.Name == "Lunch").Id,
                Capacity = 40
            };
            var viewResult = _controller.Create(model);
            viewResult.Wait();
            var result = viewResult.Result;

            Assert.IsFalse(_context.Sittings.Count() == originalcount + 1);
        }

        [TestMethod]
        public void CreateSittingTestDatabase_InvalidCapacity()
        {
            var cs = _context.Database.GetConnectionString();
            var r = _context.Restaurants.FirstOrDefault();
            var originalcount = _context.Sittings.Count();
            var model = new BeanSceneProject.Areas.Staff.Models.Sitting.Create
            {
                StartDate = DateTime.Today.AddHours(24),
                OpenTime = DateTime.Today.AddHours(32),
                CloseTime = DateTime.Today.AddHours(35),
                RestuarantId = _context.Restaurants.First().Id,
                SittingTypeId = _context.SittingTypes.FirstOrDefault(st => st.Name == "Lunch").Id,
                Capacity = -40
            };
            var viewResult = _controller.Create(model);
            viewResult.Wait();
            var result = viewResult.Result;

            Assert.IsFalse(_context.Sittings.Count() == originalcount + 1);
        }

        [TestMethod]
        public void CreateSittingTestDatabase_CloseTimeBeforeOpenTime()
        {
            var cs = _context.Database.GetConnectionString();
            var r = _context.Restaurants.FirstOrDefault();
            var originalcount = _context.Sittings.Count();
            var model = new BeanSceneProject.Areas.Staff.Models.Sitting.Create
            {
                StartDate = DateTime.Today.AddHours(24),
                OpenTime = DateTime.Today.AddHours(35),
                CloseTime = DateTime.Today.AddHours(32),
                RestuarantId = _context.Restaurants.First().Id,
                SittingTypeId = _context.SittingTypes.FirstOrDefault(st => st.Name == "Lunch").Id,
                Capacity = 40
            };
            var viewResult = _controller.Create(model);
            viewResult.Wait();
            var result = viewResult.Result;

            Assert.IsFalse(_context.Sittings.Count() == originalcount + 1);
        }

        [TestMethod]
        public void CreateSittingTestDatabase_RestaurantIdNull()
        {
            var cs = _context.Database.GetConnectionString();
            var r = _context.Restaurants.FirstOrDefault();
            var originalcount = _context.Sittings.Count();
            var model = new BeanSceneProject.Areas.Staff.Models.Sitting.Create
            {
                StartDate = DateTime.Today.AddHours(24),
                OpenTime = DateTime.Today.AddHours(32),
                CloseTime = DateTime.Today.AddHours(35),
                RestuarantId = 0,
                SittingTypeId = _context.SittingTypes.FirstOrDefault(st => st.Name == "Lunch").Id,
                Capacity = 40
            };
            var viewResult = _controller.Create(model);
            viewResult.Wait();
            var result = viewResult.Result;

            Assert.IsFalse(_context.Sittings.Count() == originalcount + 1);
        }

        [TestMethod]
        public void GetSittingDetailsTestDatabase()
        {
            var sitting = _context.Sittings
                .Include(s => s.SittingType)
                .Include(s => s.Reservations)
                .ThenInclude(r => r.Tables)
                .First();
            var model = new BeanSceneProject.Areas.Staff.Models.Sitting.Details
            {
                Sitting = sitting,
                Id = sitting.Id,
                Open = sitting.Open,
                Close = sitting.Close,
                IsClosed = sitting.IsClosed,
                Capacity = sitting.Capacity,
                Heads = sitting.Heads,
                SittingType = sitting.SittingType.Name,
                Reservations = sitting.Reservations.Count(),
                BookedTables = String.Join(", ", sitting.Reservations.Select(r => r.Tables.Select(t => t.Name)))
            };
            var viewResult = _controller.Details(model.Sitting.Id);
            viewResult.Wait();
            var result = viewResult.Result;

            var sittingResullt = _context.Sittings.FirstOrDefaultAsync(s => s.Id == model.Sitting.Id);
            sittingResullt.Wait();

            Assert.IsNotNull(result);
            Assert.IsNotNull(sittingResullt);
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

        [TestMethod]
        public void DeleteSittingTestDatabase()
        {
            var sitting = _context.Sittings.First();
            var originalcount = _context.Sittings.Count();
            var model = new BeanSceneProject.Areas.Staff.Models.Sitting.Delete
            {
                Sitting = sitting,
                Id = sitting.Id,
                Open = sitting.Open,
                Close = sitting.Close,
                Capacity = 42,
                IsClosed = true
            };
            var viewResult = _controller.Delete(model.Sitting.Id, model);
            viewResult.Wait();
            Assert.IsTrue(_context.Sittings.Count() == originalcount - 1);
        }
    }
}
