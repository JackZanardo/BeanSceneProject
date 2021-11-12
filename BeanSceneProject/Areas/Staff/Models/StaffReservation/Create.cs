using BeanSceneProject.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Areas.Staff.Models.StaffReservation
{
    public class Create
    {
        public int? SittingId { get; set; }
        public string SittingInfo { get; set; }
        public DateTime SittingOpen { get; set; }
        public DateTime SittingClose { get; set; }

        [Required(ErrorMessage = "Start time required")]
        public string Start { get; set; }
        public int Available { get; set; }

        [Display(Name = "Customer Number")]
        [Required(ErrorMessage = "Customer Number required")]
        public int CustomerNum { get; set; }

        [Required(ErrorMessage = "Duration requireed")]
        public int Duration { get; set; }
        public string Notes { get; set; }

        [Display(Name = "Origin")]
        [Required(ErrorMessage = "Reservation origin required")]
        public int ReservationOriginId { get; set; }
        public SelectList ReservationOrigins { get; set; }

        [Display(Name = "Customer First Name")]
        [Required(ErrorMessage = "First Name required")]
        public string FirstName { get; set; }

        [Display(Name = "Customer Last Name")]
        [Required(ErrorMessage = "Last Name required")]
        public string LastName { get; set; }

        [Display(Name = "Customer Email Address")]
        [Required(ErrorMessage = "Email required")]
        public string Email { get; set; }

        [Display(Name = "Customer Phone number")]
        [Required(ErrorMessage = "Phone Number required")]
        public string MobileNumber { get; set; }
    }
}
