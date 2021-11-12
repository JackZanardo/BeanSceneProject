using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Areas.Staff.Models.StaffReservation
{
    public class WalkIn
    {
        public int? SittingId { get; set; }
        public string SittingInfo { get; set; }
        public DateTime SittingOpen { get; set; }
        public DateTime SittingClose { get; set; }
        public int Available { get; set; }

        [Display(Name = "Customer Number")]
        [Required(ErrorMessage = "Customer Number required")]
        public int CustomerNum { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Duration requireed")]
        public int Duration { get; set; }
    }
}
