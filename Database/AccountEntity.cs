using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace QL_HS.Database
{
    [Table(name:"Accounts")]
    public partial class AccountEntity: BaseEntity
    {
        [Column(TypeName ="NVARCHAR(20)")]
        public string Username { get; set; }
        [Column(TypeName = "NVARCHAR(200)")]
        [JsonIgnore]
        public string Password { get; set; }
        [Column(TypeName = "NVARCHAR(100)")]
        public string Role { get; set; }
        [JsonIgnore]
        public virtual GuardianEntity? Guardian { get; set; }
    }
}
