using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Logitun.Core.Entities
{
    [Table("auth_role")]
    public class Role
    {
        [Key]
        [MaxLength(36)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<Credentials> Credentials { get; set; } = new List<Credentials>();


    }
}