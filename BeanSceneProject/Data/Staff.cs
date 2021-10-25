using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Data
{
    public class StaffMember: Person
    {
        public int RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; }
        public int StaffTypeId { get; set; }
        public StaffType StaffType { get; set; }
    }
}
