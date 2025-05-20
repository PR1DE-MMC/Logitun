using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logitun.Shared.DTOs
{
    public class TruckDto
    {
        public int TruckId { get; set; }
        public string PlateNumber { get; set; }
        public string Model { get; set; }
        public int? ManufactureYear { get; set; }
        public decimal? CapacityKg { get; set; }
        public string FuelType { get; set; }
        public DateTime? LastMaintenanceDate { get; set; }
    }
}
