using Logitun.Core.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Logitun.Core.Entities
{
    [Table("missions")]
    public class Mission
    {
        [Key]
        [Column("mission_id")]
        public int MissionId { get; set; }

        [Column("truck_id")]
        public int? TruckId { get; set; }

        [Column("driver_id")]
        public Guid? DriverId { get; set; }

        [Column("origin_location_id")]
        public int? OriginLocationId { get; set; }

        [Column("destination_location_id")]
        public int? DestinationLocationId { get; set; }

        [Required]
        [Column("start_datetime")]
        public DateTime StartDatetime { get; set; }

        [Column("end_datetime")]
        public DateTime? EndDatetime { get; set; }

        [Column("distance_km", TypeName = "decimal(10,2)")]
        public decimal? DistanceKm { get; set; }

        [Column("payload_weight", TypeName = "decimal(10,2)")]
        public decimal? PayloadWeight { get; set; }

        [MaxLength(50)]
        [Column("status")]
        public string Status { get; set; } = "SCHEDULED";

        // Navigation properties
        [ForeignKey("TruckId")]
        public virtual Truck? Truck { get; set; }

        [ForeignKey("DriverId")]
        public virtual User? Driver { get; set; }

        [ForeignKey("OriginLocationId")]
        public virtual Location? OriginLocation { get; set; }

        [ForeignKey("DestinationLocationId")]
        public virtual Location? DestinationLocation { get; set; }
    }
}