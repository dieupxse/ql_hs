using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace QL_HS.Database
{
    [Table(name: "Pickups")]
    public partial class PickupEntity: BaseEntity
    {
        public DateTime Date { get; set; }
        public int StudentId { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string State { get; set; } = "REQUEST";
        public virtual StudentEntity Student { get; set; }
        public int GuardianId { get; set; }
        public virtual GuardianEntity Guardian { get; set; }
    }
}
