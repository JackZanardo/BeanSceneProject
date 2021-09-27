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

        public async Task<Person> FindOrCreatePerson(string email, string fName, string lName, string mobileNumber)
        {
            var person = await _context.People.FirstOrDefaultAsync(p => p.Email == email);
            if (person == null)
            {
                person = new Person
                {
                    Email = email,
                    FirstName = fName,
                    LastName = lName,
                    MobileNumber = mobileNumber
                };
                _context.People.Add(person);
                await _context.SaveChangesAsync();
                return person;
            }
            return person;

        }
    }
}
