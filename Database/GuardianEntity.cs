using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QL_HS.Database
{
    [Table(name: "Guardians")]
    public partial class GuardianEntity: BaseEntity
    {
        [Column(TypeName ="NVARCHAR(200)")]
        public string Name { get; set; }
        [Column(TypeName = "NVARCHAR(20)")]
        [Required]
        public string Phone { get; set; }
        [Column(TypeName = "NVARCHAR(300)")]
        public string Address { get; set; }
        [Column(TypeName = "NVARCHAR(500)")]
        public string Description { get; set; }
        public int? AccountId { get; set; }
        public virtual ICollection<StudentEntity> Students { get; set; }
        public virtual ICollection<PickupEntity> Pickups { get; set; }
        [JsonIgnore]
        public virtual AccountEntity? Account { get; set;}
    }
}
