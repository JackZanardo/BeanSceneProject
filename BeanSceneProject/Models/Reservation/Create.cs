﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Models.Reservation
{
    public class Create
    {
        public DateTime Start { get; set; }
        public int SittingId { get; set; }
        public SelectList StartTimes { get; set; }
        public int CustomerNum { get; set; }
        public int Duration { get; set; }
        public string Notes { get; set; }

        public int AreaId { get; set; }
        public SelectList Areas { get; set; }

        [Required(ErrorMessage = "First Name required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number required")]
        public string MobileNumber { get; set; }
    }
}
