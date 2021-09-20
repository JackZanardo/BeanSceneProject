using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Data
{
    public enum ReservationStatus { Pending = 1, Confirmed = 2, Seated = 3, Complete = 4, Cancelled = 5 }
    public class Reservation
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public int CustomerNum { get; set; }
        public int Duration { get; set; }
        public DateTime End { get => Start.AddMinutes(Duration);}
        public string Notes { get; set; }
        public ReservationStatus ReservationStatus { get; set; }
        public int ReservationOriginId { get; set; }
        public ReservationOrigin ReservationOrigin { get; set; }
        public int SittingId { get; set; }
        public Sitting Sittings { get; set; }
        public int PersonId { get; set; }
        public Person Person { get; set; }
    }
}
