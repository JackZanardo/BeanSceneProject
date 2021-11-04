using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Areas.Staff.Models.Staff
{
    public class Create
    {
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name required")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name required")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email required")]
        public string Email { get; set; }

        [Display(Name = "Staff Type")]
        [Required(ErrorMessage = "Staff type required")]
        public int StaffTypeId { get; set; }
        public string StaffTypes { get; set; }

        [Display(Name = "Restuarant")]
        [Required(ErrorMessage = "Restuarant Required")]
        public int RestuarantId { get; set; }
        public SelectList Restraunts { get; set; }

        public bool BulkCreate { get; set; }
    }
}
