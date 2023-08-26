using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace QL_HS.Database
{
    [Table(name:"Students")]
    public partial class StudentEntity: BaseEntity
    {
        [Column(TypeName = "NVARCHAR(300)")]
        public string Name { get; set; }
        public string Dob { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string Class { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string Grade { get; set; }
        [Column(TypeName ="NVARCHAR(1000)")]
        public string Bio { get; set; }
        public int GuardianId { get; set; }
        public virtual GuardianEntity Guardian { get; set; }
        public virtual ICollection<PickupEntity> Pickups { get; set; }
    }
}
