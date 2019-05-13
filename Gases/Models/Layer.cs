using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Gases.Models
{
    public class Layer
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "GeoServerName")]
        public string GeoServerName { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "FileNameWithPath")]
        public string FileNameWithPath { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "GeoServerStyle")]
        public string GeoServerStyle { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NameKK")]
        public string NameKK { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NameRU")]
        public string NameRU { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NameEN")]
        public string NameEN { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Name")]
        public string Name
        {
            get
            {
                string language = new RequestLocalizationOptions().DefaultRequestCulture.Culture.Name,
                    name = NameRU;
                if (language == "kk")
                {
                    name = NameKK;
                }
                if (language == "ru")
                {
                    name = NameRU;
                }
                if (language == "en")
                {
                    name = NameEN;
                }
                return name;
            }
        }

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

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Year")]
        public int Year { get; set; }
    }
}
