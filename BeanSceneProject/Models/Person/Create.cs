using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BeanSceneProject.Models.Person
{
    public class Create
    {
        [Required(ErrorMessage = "First Name required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage ="Last Name required")]
        public string LastName { get; set; }

        [Required(ErrorMessage ="Email required")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Phone Number required")]
        public string MobileNumber { get; set; }
    }
}
