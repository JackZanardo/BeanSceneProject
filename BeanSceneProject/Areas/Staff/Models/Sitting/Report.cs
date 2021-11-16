using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Areas.Staff.Models.Sitting
{
    public class Report
    {
        public int Id { get; set; }
        [Display(Name = "People Booked")]
        public int Heads { get; set; }
        [Display(Name = "Number of Currently Booked Tables")]
        public string BookedTables { get; set; }

        [Display(Name = "Number of Total Reservations")]
        public int TotalReservations { get; set; }
        [Display(Name = "Number of Pending Reservations")]
        public int PendingReservations { get; set; }
        [Display(Name = "Number of Confirmed Reservations")]
        public int ConfirmedReservations { get; set; }
        [Display(Name = "Number of Completed Reservations")]
        public int CompletedReservations { get; set; }
        [Display(Name = "Number of Cancelled Reservations")]
        public int CancelledReservations { get; set; }
    }
}
