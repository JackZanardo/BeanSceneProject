using BeanSceneProject.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.DTOs
{
    public class ReservationDto
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public int CustomerNum { get; set; }
        public int Duration { get; set; }
        public DateTime End { get => Start.AddMinutes(Duration); }
        public string Notes { get; set; }
        public string ReservationStatus { get; set; }
        public string ReservationOrigin { get; set; }
        public int SittingId { get; set; }
        public int PersonId { get; set; }
        public Person Person { get; set; }
        public List<TableDto> Tables { get; set; }
    }
}
