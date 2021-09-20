using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Data
{
    public class Area
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<Table> Tables { get; set; } = new List<Table>();
        public int RestuarantId { get; set; }
        public Restaurant Restaurant { get; set; }
    }
}
