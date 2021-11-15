using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Data
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
        public List<Area> Areas { get; set; } = new List<Area>();
        public List<Sitting> Sittings { get; set; } = new List<Sitting>();
        public List<Person> Staff { get; set; } = new List<Person>();
    }
}
