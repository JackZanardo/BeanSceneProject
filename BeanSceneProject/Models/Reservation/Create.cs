using BeanSceneProject.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Models.Reservation
{
    public class Create
    {
        [Display(Name = "Date")]
        public string StartDates { get; set; }

        [Display(Name = "Start Time")]
        [Required(ErrorMessage = "Please select a start time")]
        public string StartTime { get; set; }

        [Display(Name = "Session")]
        [Required(ErrorMessage = "Please select a session")]
        public int SittingId { get; set; }

        [Display(Name = "Number of People")]
        [Required(ErrorMessage = "Please enter number of people")]
        public int CustomerNum { get; set; }

        [Required(ErrorMessage = "Please select a duration")]
        public int Duration { get; set; }
        public string Notes { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name required")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name required")]
        public string LastName { get; set; }

        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Email required")]
        public string Email { get; set; }

        [Display(Name = "Phone number")]
        [Required(ErrorMessage = "Phone Number required")]
        public string MobileNumber { get; set; }
    }
}
