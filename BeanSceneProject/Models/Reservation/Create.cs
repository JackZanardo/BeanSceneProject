using BeanSceneProject.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        [Required(ErrorMessage ="Please select a time.")]
        public DateTime Start { get; set; }
        [Required(ErrorMessage ="Please select an area.")]
        public int SittingId { get; set; }
        [Required(ErrorMessage ="Please enter number of guests.")]
        public int CustomerNum { get; set; }
        public string Notes { get; set; }
        public int ReservationOriginId { get; set; }
        public SelectList ReservationOrigin { get; set; }

    }
}
