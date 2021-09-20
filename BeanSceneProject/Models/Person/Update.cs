using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Models.Person
{
    public class Update : Create
    {
        [Required]
        public int Id { get; set; }
    }
}
