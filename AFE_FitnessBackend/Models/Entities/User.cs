using AFE_FitnessBackend.Models.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AFE_FitnessBackend.Models.Entities
{
    public class User
    {
        [Key]
        public long UserId { get; set; }
        [MaxLength(64)]
        public string FirstName { get; set; }
        [MaxLength(32)]
        public string LastName { get; set; }
        [MaxLength(254)]
        public string Email { get; set; }
        [MaxLength(60)]
        public string PwHash { get; set; }
        [Column(TypeName = "nvarchar(20)")]  // Store enum as string in Db
        public Role AccountType { get; set; }
        public long? PersonalTrainerId { get; set; }
        [ForeignKey("PersonalTrainerId")]
        public virtual ICollection<User> Clients { get; set; }
    }
}
