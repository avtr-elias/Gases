using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Gases.Models
{
    public class GData
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "GDataType")]
        public GDataType GDataType { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "GDataType")]
        public int GDataTypeId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Gase")]
        public Gase Gase { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Gase")]
        public int GaseId { get; set; }
        
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "VerticalSlice")]
        public decimal VerticalSlice { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Region")]
        public Region Region { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Region")]
        public int? RegionId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Longtitude")]
        public decimal? Longtitude { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Latitude")]
        public decimal? Latitude { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Value")]
        public decimal? Value { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Year")]
        public int Year { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Month")]
        public int? Month { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Season")]
        public Season? Season { get; set; }
    }

    public enum Season
    {
        Winter = 1,
        Spring = 2,
        Summer = 3,
        Autumn = 4
    }

    public class GDataIndexPageViewModel
    {
        public IEnumerable<GData> Items { get; set; }
        public Pager Pager { get; set; }
    }
}
