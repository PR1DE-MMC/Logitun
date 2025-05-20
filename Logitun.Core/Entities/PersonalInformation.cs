using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Logitun.Core.Entities
{
    [Table("auth_information")]
    public class PersonalInformation
    {
        [Key]
        [Column("information_id")]
        public int InformationId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Column("last_name")]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(8)]
        [Column("cin")]
        public string? Cin { get; set; }

        [Required]
        [Column("birthdate")]
        public DateTime Birthdate { get; set; }

        [Required]
        [MaxLength(8)]
        [Column("phone_number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(256)]
        [Column("email")]
        public string? Email { get; set; }

        // Navigation properties
        public virtual User? User { get; set; }
    }
}