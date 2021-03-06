using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeanSceneProject.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationOrigin> ReservationOrigins { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Sitting> Sittings { get; set; }
        public DbSet<SittingType> SittingTypes { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Person> People { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Restaurant>()
                .OwnsOne(r => r.Address);

            builder.Entity<Reservation>()
                .HasOne(r => r.Sitting)
                .WithMany(s => s.Reservations)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Reservation>()
                .HasOne(r => r.Person)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Sitting>()
                .HasMany(s => s.Reservations)
                .WithOne(r => r.Sitting)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Sitting>()
                .HasOne(s => s.SittingType)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Sitting>()
                .HasOne(s => s.Restaurant)
                .WithMany(r => r.Sittings)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Person>()
                .Property("UserId")
                .HasMaxLength(450);

            builder.Entity<Person>()
                .HasOne<IdentityUser>()
                .WithOne()
                .HasForeignKey("Person", "UserId")
                .IsRequired(false);

            builder.Entity<SittingType>()
                .Property("DefaultOpenTime")
                .IsRequired(false);
                                              
            builder.Entity<SittingType>()
                .Property("DefaultCloseTime")
                .IsRequired(false);
        }
    }
}
