using BeanSceneProject.Controllers;
using BeanSceneProject.Data;
using BeanSceneProject.Services;
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
        private ApplicationDbContext _dbContext;

        public ReservationControllerTest()
        {
            /*_dbContext = new ApplicationDbContext(options =>
                options.UseSqlServer(@"Server=localhost;Database=BeanSceneDatabase;Trusted_Connection=True;MultipleActiveResultSets=true")
            );*/
        }
    }
}
