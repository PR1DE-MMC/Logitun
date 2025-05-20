using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Logitun.Core.Entities
{
    [Table("time_off_requests")]
    public class TimeOffRequest
    {
        [Key]
        [Column("request_id")]
        public int RequestId { get; set; }

        [Column("driver_id")]
        public Guid? DriverId { get; set; }

        [Required]
        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [MaxLength(50)]
        [Column("status")]
        public string Status { get; set; } = "PENDING";

        // Navigation properties
        [ForeignKey("DriverId")]
        public virtual User? Driver { get; set; }
    }
}