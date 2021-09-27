using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Areas.Staff.Models.Reservation
{
    public class Create
    {
        public DateTime Start { get; set; }
        public int CustomerNum { get; set; }
        public int Duration { get; set; }
        public string Notes { get; set; }
        public int ReservationOriginId { get; set; }
        public SelectList ReservationOrigins { get; set; }
        public int SittingId { get; set; }
        public SelectList Sittings { get; set; }
        public int AreaId { get; set; }
        public SelectList Areas { get; set; }
        public int TableId { get; set; }
        public SelectList Tables { get; set; }
        public int PersonId { get; set; }
        public SelectList Person { get; set; }
    }
}
