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
    }
}
