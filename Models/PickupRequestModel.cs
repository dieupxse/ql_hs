using System.ComponentModel.DataAnnotations;

namespace QL_HS.Models
{
    public class PickupRequestModel
    {
        [Required]
        public int GuardianId { get; set; }
        [Required]
        public int StudentId { get; set; }
    }
}
