using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BeanSceneProject.Data;

namespace BeanSceneProject.Areas.Staff.Models.Sitting
{
    public class Update
    {
        public BeanSceneProject.Data.Sitting Sitting { get; set; }
        [Required]
        public DateTime Open { get; set; }
        [Required]
        public DateTime Close { get; set; }
        [Required]
        public int Capacity { get; set; }
        [Required]
        public int SittingTypeId { get; set; }
        public SelectList SittingTypes { get; set; }
        [Required]
        public int RestuarantId { get; set; }
        public SelectList Restraunts { get; set; }

        public bool IsClosed { get; set; }
        public SelectList IsClosedSelect { get; set; }
    }
}
