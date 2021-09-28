﻿using BeanSceneProject.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Services
{
    public class PersonService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PersonService(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<Person> UpsertPersonAsync(Person data, bool update)
        {
            var person = await _context.People.FirstOrDefaultAsync(p => p.Email == data.Email);
            if (person == null)
            {
                person = new Person
                {
                    Email = data.Email,
                    FirstName = data.FirstName,
                    LastName = data.LastName,
                    MobileNumber = data.MobileNumber
                };
                _context.People.Add(person);
            }
            if (person != null && update)
            {
                person.Email = data.Email;
                person.FirstName = data.FirstName;
                person.LastName = data.LastName;
                person.MobileNumber = data.MobileNumber;
            }
            await _context.SaveChangesAsync();
            return person;
        }
    }
}