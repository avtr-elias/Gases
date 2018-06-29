using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Gases.Models
{
    public class Gase
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Name")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Formula")]
        public string Formula { get; set; }
    }
}
