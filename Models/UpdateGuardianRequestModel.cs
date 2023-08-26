using System.ComponentModel.DataAnnotations;

namespace QL_HS.Models
{
    public class UpdateGuardianRequestModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Address { get; set; }
        public string Description { get; set; }

    }
}
