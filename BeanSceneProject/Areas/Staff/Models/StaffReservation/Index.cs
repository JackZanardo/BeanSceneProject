using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeanSceneProject.Data;

namespace BeanSceneProject.Areas.Staff.Models.StaffReservation
{
    public class Index
    {
        public int? SittingId { get; set; }
        public string SittingInfo { get; set; }
        public bool OpenWalkIn { get; set; }
        public IEnumerable<Reservation> Reservations { get; set; }
    }
}
