using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Logitun.Core.Entities
{
    [Table("auth_user")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public Guid UserId { get; set; }

        [Required]
        [Column("information_id")]
        public int InformationId { get; set; }

        [Required]
        [Column("credentials_id")]
        public int CredentialsId { get; set; }

        // Navigation properties
        [ForeignKey("InformationId")]
        public virtual PersonalInformation Information { get; set; } = null!;

        [ForeignKey("CredentialsId")]
        public virtual Credentials Credentials { get; set; } = null!;

        public virtual ICollection<TimeOffRequest> TimeOffRequests { get; set; } = new List<TimeOffRequest>();
        public virtual ICollection<Mission> Missions { get; set; } = new List<Mission>();
    }
}
