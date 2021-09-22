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
        [Required]
        public DateTime Open { get; set; }
        [Required]
        public DateTime Close { get; set; }
        [Required]
        public int Capacity { get; set; }
        [Required]
        public int SittingTypeId { get; set; }
        public SelectList SittingTypes { get; set; }

    }
}
