using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Data
{
    public class SittingType
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string DefaultOpenTime { get; set; }
        public string DefaultCloseTime { get; set; }
    }
}
