using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Data
{
    public class Sitting
    {
        public int Id { get; set; }
        public DateTime Open { get; set; }
        public DateTime Close { get; set; }
        public bool IsClosed { get; set; }
        public int Capacity { get; set; }
        public int Heads { get => Reservations.Sum(r=>r.CustomerNum); }
        public int Available { get => Capacity - Heads; }

        //Relationships
        public int RestuarantId { get; set; }
        public Restaurant Restaurant { get; set; }
        public int SittingTypeId { get; set; }
        public SittingType SittingType { get; set; }
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
