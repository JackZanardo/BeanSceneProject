using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Areas.Staff.Models.Sitting
{
    public class Create
    {
        [Display(Name = "Sitting Date")]
        [Required(ErrorMessage = "Sitting Date required")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Open Time")]
        [Required(ErrorMessage = "Open time required")]
        public DateTime OpenTime { get; set; }

        [Display(Name = "Close Time")]
        [Required(ErrorMessage = "Close time required")]
        public DateTime CloseTime { get; set; }

        [Required(ErrorMessage = "Capacity required")]
        public int Capacity { get; set; }

        [Display(Name = "Sitting Type")]
        [Required(ErrorMessage = "Sitting type required")]
        public int SittingTypeId { get; set; }
        public string SittingTypes { get; set; }

        [Display(Name = "Restuarant")]
        [Required(ErrorMessage = "Restuarant Required")]
        public int RestuarantId { get; set; }
        public SelectList Restraunts { get; set; }

        public bool BulkCreate { get; set; }

    }
}
