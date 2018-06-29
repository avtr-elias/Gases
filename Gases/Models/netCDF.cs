using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Gases.Models
{
    public class NetCDF
    {
        public int Id { get; set; }

        public int GaseId { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Gase")]
        public Gase Gase { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Date")]
        public DateTime Date { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Name")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Unit")]
        public string Unit { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Longtitude")]
        public decimal Longtitude { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Latitude")]
        public decimal Latitude { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Value")]
        public decimal Value { get; set; }
    }
}
