using BeanSceneProject.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Areas.Member.Models
{
    public class History
    {
        public Person Person { get; set; }
        public IEnumerable<Reservation> Reservations { get; set; }
    }
}
