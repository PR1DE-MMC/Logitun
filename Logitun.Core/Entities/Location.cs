using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Logitun.Core.Entities
{
    [Table("locations")]
    public class Location
    {
        [Key]
        [Column("location_id")]
        public int LocationId { get; set; }

        [MaxLength(100)]
        [Column("name")]
        public string? Name { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("address")]
        public string Address { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<Mission> OriginMissions { get; set; } = new List<Mission>();
        public virtual ICollection<Mission> DestinationMissions { get; set; } = new List<Mission>();
    }
}