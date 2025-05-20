using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Logitun.Core.Entities
{
    [Table("trucks")]
    public class Truck
    {
        [Key]
        [Column("truck_id")]
        public int TruckId { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("plate_number")]
        public string PlateNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("model")]
        public string Model { get; set; } = string.Empty;

        [Column("manufacture_year")]
        public int? ManufactureYear { get; set; }

        [Column("capacity_kg", TypeName = "decimal(10,2)")]
        public decimal? CapacityKg { get; set; }

        [MaxLength(20)]
        [Column("fuel_type")]
        public string? FuelType { get; set; }

        [Column("last_maintenance_date")]
        public DateTime? LastMaintenanceDate { get; set; }

        // Navigation properties
        public virtual ICollection<Mission> Missions { get; set; } = new List<Mission>();
    }
}