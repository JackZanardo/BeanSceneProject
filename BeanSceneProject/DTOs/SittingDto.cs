using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.DTOs
{
    public class SittingDto
    {
        public int Id { get; set; }
        public DateTime Open { get; set; }
        public DateTime Close { get; set; }
        public bool IsClosed { get; set; }
        public int Available { get; set; }
        public int Capacity { get; set; }
        public int RestaurantId { get; set; }
        public RestaurantDto Restaurant { get; set; }
        public int SittingTypeId { get; set; }
        public SittingTypeDto SittingType { get; set; }
    }
}
