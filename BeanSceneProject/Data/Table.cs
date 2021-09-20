using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Data
{
    public class Table
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AreaId { get; set; }
        public Area Area { get; set; }
    }
}
