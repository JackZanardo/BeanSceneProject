using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Models.Reservation
{
    public class Session
    {
        public int Id { get; set; }
        public string InfoText { get; set; }
        public DateTime Start { get; set; }
        public int Duration { get; set; }
    }
}
