using BeanSceneProject.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Models
{
    public class SeedData
    {
        private IServiceProvider _serviceProvider;
        public SeedData(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public void Initialize()
        {
            ReservationOriginInit(_serviceProvider);
            SittingTypeInit(_serviceProvider);
            RestaurantInit(_serviceProvider);
            CreateRoles(_serviceProvider);
            SeedAdmin(_serviceProvider);
            SeedStaff(_serviceProvider);
        }

        private void ReservationOriginInit(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                var cs = context.Database.GetConnectionString();
                if (context.ReservationOrigins.Any())
                {
                    return;
                }
                List<ReservationOrigin> reservationOrigins = new List<ReservationOrigin>();
                reservationOrigins.Add(new ReservationOrigin
                {
                    Name = "Website"
                });
                reservationOrigins.Add(new ReservationOrigin
                {
                    Name = "Phone"
                });
                reservationOrigins.Add(new ReservationOrigin
                {
                    Name = "Email"
                });
                reservationOrigins.Add(new ReservationOrigin
                {
                    Name = "InPerson"
                });
                context.ReservationOrigins.AddRangeAsync(reservationOrigins);
                context.SaveChanges();
            }
        }

        private void SittingTypeInit(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                if (context.SittingTypes.Any())
                {
                    return;
                }
                List<SittingType> sittingTypes = new List<SittingType>();
                sittingTypes.Add(new SittingType
                {
                    Name = "Breakfast",
                    DefaultOpenTime = "08:00",
                    DefaultCloseTime = "10:30"
                });
                sittingTypes.Add(new SittingType
                {
                    Name = "Lunch",
                    DefaultOpenTime = "11:30",
                    DefaultCloseTime = "16:00"
                });
                sittingTypes.Add(new SittingType
                {
                    Name = "Dinner",
                    DefaultOpenTime = "17:00",
                    DefaultCloseTime = "21:00"
                });
                sittingTypes.Add(new SittingType
                {
                    Name = "Special",
                    DefaultOpenTime = "08:00",
                    DefaultCloseTime = "17:00"
                });
                context.SittingTypes.AddRangeAsync(sittingTypes);
                context.SaveChanges();
            }
        }

        private void RestaurantInit(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                if (context.Restaurants.Any())
                {
                    return;
                }
                var address = new Address
                {
                    StreetNumber = "27",
                    StreetName = "Crystal Street",
                    Suburb = "Petersham",
                    State = "NSW",
                    PostCode = 2049
                };
                var areas = new List<Area>();
                areas.Add(new Area
                {
                    Name = "Main",
                    Tables = GetTables('M',10)
                });
                areas.Add(new Area
                {
                    Name = "Outside",
                    Tables = GetTables('O', 10)
                });
                areas.Add(new Area
                {
                    Name = "Balcony",
                    Tables = GetTables('B', 10)
                });
                var restaurant = new Restaurant
                {
                    Name = "Bean Scene Petersham",
                    Address = address,
                    Areas = areas
                };
                context.Restaurants.AddAsync(restaurant);
                context.SaveChanges();
            }
        }

        private List<Table> GetTables(char areaCode, int tableNum)
        {
            var tables = new List<Table>();
            for(int i = 1; i <= tableNum; i++)
            {
                tables.Add(new Table
                {
                    Name = $"{areaCode}{i}"
                });
            }
            return tables;
        }

        private void CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roleNames = { "Admin", "Member", "Staff" };
            foreach (var roleName in roleNames)
            {
                Task<bool> roleExists = roleManager.RoleExistsAsync(roleName);
                roleExists.Wait();
                if (!roleExists.Result)
                {
                    Task<IdentityResult> result = roleManager.CreateAsync(new IdentityRole(roleName));
                    result.Wait();
                }
            }
        }

        private void SeedAdmin(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            List<IdentityUser> baseAdmins = new List<IdentityUser>();
            baseAdmins.Add(new IdentityUser
            {
                UserName = "Admin1",
                Email = "SuperCombatWombat@protonmail.com",
                EmailConfirmed = true
            });
            baseAdmins.Add(new IdentityUser
            {
                UserName = "Admin2",
                Email = "vincentrosslee@gmail.com",
                EmailConfirmed = true
            });
            baseAdmins.Add(new IdentityUser
            {
                UserName = "Admin3",
                Email = "Manager@Beanscene.com",
                EmailConfirmed = true
            });
            foreach (var baseAdmin in baseAdmins)
            {
                Task<IdentityUser> userExists = userManager.FindByEmailAsync(baseAdmin.Email);
                userExists.Wait();
                if (userExists.Result == null)
                {
                    Task<IdentityResult> result = userManager.CreateAsync(baseAdmin, "Admin*123");
                    result.Wait();
                    if (result.Result.Succeeded)
                    {
                        userManager.AddToRoleAsync(baseAdmin, "Admin").Wait();
                    }
                }
            }

        }

        private void SeedStaff(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            List<IdentityUser> baseStaff = new List<IdentityUser>();
            baseStaff.Add(new IdentityUser
            {
                UserName = "Staff1",
                Email = "Staff1@Beanscene.com",
                EmailConfirmed = true
            });
            foreach (var bs in baseStaff)
            {
                Task<IdentityUser> userExists = userManager.FindByEmailAsync(bs.Email);
                userExists.Wait();
                if (userExists.Result == null)
                {
                    Task<IdentityResult> result = userManager.CreateAsync(bs, "Staff*123");
                    result.Wait();
                    if (result.Result.Succeeded)
                    {
                        userManager.AddToRoleAsync(bs, "Staff").Wait();
                    }
                }
            }
        }
    }
}
