using BeanSceneProject.Data;
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
        public static void Initialize(IServiceProvider serviceProvider)
        {
            ReservationOriginInit(serviceProvider);
            SittingTypeInit(serviceProvider);
            RestaurantInit(serviceProvider);
        }

        public static void ReservationOriginInit(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
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

        public static void SittingTypeInit(IServiceProvider serviceProvider)
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
                    Name = "Breakfast"
                });
                sittingTypes.Add(new SittingType
                {
                    Name = "Lunch"
                });
                sittingTypes.Add(new SittingType
                {
                    Name = "Dinner"
                });
                context.SittingTypes.AddRangeAsync(sittingTypes);
                context.SaveChanges();
            }
        }

        public static void RestaurantInit(IServiceProvider serviceProvider)
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

        public static List<Table> GetTables(char areaCode, int tableNum)
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
    }
}
