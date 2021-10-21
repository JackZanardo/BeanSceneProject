using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Areas.Staff.Models.Sitting
{
    public class Delete
    {
        public int Id { get; set; }
        [Display(Name = "Open Time")]
        public DateTime Open { get; set; }
        [Display(Name = "Close Time")]
        public DateTime Close { get; set; }
        [Display(Name = "Staus")]
        public bool IsClosed { get; set; }
        [Display(Name = "Capacity")]
        public int Capacity { get; set; }
        [Display(Name = "People Booked")]
        public int Heads { get; set; }
        [Display(Name = "Number of Reservations")]
        public int Reservations { get; set; }
        [Display(Name = "Booked Tables")]
        public List<string> BookedTables { get; set; }
        [Display(Name = "Restuarant")]
        public string Restuarant { get; set; }
        [Display(Name = "Sitting Type")]
        public string SittingType { get; set; }
    }
}
