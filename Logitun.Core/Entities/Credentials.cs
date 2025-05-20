
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Logitun.Core.Entities
{
    [Table("auth_credentials")]
    public class Credentials
    {
        [Key]
        [Column("credential_id")]
        public int CredentialId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("login")]
        public string Login { get; set; } = string.Empty;

        [Required]
        [MaxLength(256)]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [Column("activated")]
        public bool Activated { get; set; } = false;

        [MaxLength(10)]
        [Column("lang_key")]
        public string LangKey { get; set; } = "fr";

        [MaxLength(20)]
        [Column("activation_key")]
        public string? ActivationKey { get; set; }

        [MaxLength(20)]
        [Column("reset_key")]
        public string? ResetKey { get; set; }

        [Column("reset_date")]
        public DateTime? ResetDate { get; set; }

        // Navigation properties
        public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
        public virtual User? User { get; set; }
    }
}