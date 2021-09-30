using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Areas.Staff.Models.Sitting
{
    public class Details
    {
        public int Id { get; set; }
        public DateTime Open { get; set; }
        public DateTime Close { get; set; }
        public bool IsClosed { get; set; }
        public int Capacity { get; set; }
        public int Heads { get; set; }
        public int Reservations { get; set; }
        public List<string> BookedTables { get; set; }
        public string Restuarant { get; set; }
        public string SittingType { get; set; }


    }
}
