using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Gases.Models
{
    public class GeoTiffFile
    {
        public int Id { get; set; }

        public int? GaseId { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Gase")]
        public Gase Gase { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Year")]
        public string Year { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "VerticalSlice")]
        public decimal? VerticalSlice { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Name")]
        public string Name { get; set; }
    }
}
