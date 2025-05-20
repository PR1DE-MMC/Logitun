using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Logitun.Shared.DTOs
{
    public class TruckDto
    {
        public int TruckId { get; set; }

        [Required(ErrorMessage = "License plate is required")]
        [StringLength(20, ErrorMessage = "License plate cannot be longer than 20 characters")]
        public string PlateNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Model is required")]
        [StringLength(50, ErrorMessage = "Model cannot be longer than 50 characters")]
        public string Model { get; set; } = string.Empty;

        [Range(1900, 2100, ErrorMessage = "Manufacture year must be between 1900 and 2100")]
        public int? ManufactureYear { get; set; }

        [Range(0, 100000, ErrorMessage = "Capacity must be between 0 and 100,000 kg")]
        public decimal? CapacityKg { get; set; }

        [Required(ErrorMessage = "Fuel type is required")]
        [StringLength(20, ErrorMessage = "Fuel type cannot be longer than 20 characters")]
        public string FuelType { get; set; } = string.Empty;

        public DateTime? LastMaintenanceDate { get; set; }
    }
}
