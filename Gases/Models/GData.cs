using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gases.Models
{
    public class GData
    {
        public int Id { get; set; }

        public GDataType GDataType { get; set; }
        public int GDataTypeId { get; set; }

        public Gase Gase { get; set; }
        public int GaseId { get; set; }

        public decimal VerticalSlice { get; set; }

        public Region Region { get; set; }
        public int? RegionId { get; set; }

        public decimal? Longtitude { get; set; }
        public decimal? Latitude { get; set; }

        public int Year { get; set; }

        public int? Month { get; set; }

        public Season Season { get; set; }
    }

    public enum Season
    {
        Winter = 1,
        Spring = 2,
        Summer = 3,
        Autumn = 4
    }
}
