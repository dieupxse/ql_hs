using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace QL_HS.Database
{
    public partial class BaseEntity
    {
        public int Id { get; set; }
        [Column(TypeName ="nvarchar(200)")]
        [JsonIgnore]
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [Column(TypeName = "nvarchar(200)")]
        [JsonIgnore]
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [JsonIgnore]
        public string Status { get; set; } = EntityStatus.ENABLE;
    }
}
