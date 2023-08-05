using System.ComponentModel.DataAnnotations;

namespace QL_HS.Models
{
    public class CreateAccountRequestModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; }
    }
}
