using System.ComponentModel.DataAnnotations;

namespace QL_HS.Models
{
    public class CreateAccountRequestModel
    {
        [Required]
        public string Username { get; set; }
        public string Password { get; set; }
        [Required]
        public string Role { get; set; }
    }
}
