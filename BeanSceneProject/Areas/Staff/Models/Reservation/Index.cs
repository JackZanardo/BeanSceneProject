using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeanSceneProject.Data;

namespace BeanSceneProject.Areas.Staff.Models.Reservation
{
    public class Index
    {
        public int? SittingId { get; set; }
        public IEnumerable<BeanSceneProject.Data.Reservation> Reservations { get; set; }
    }
}
