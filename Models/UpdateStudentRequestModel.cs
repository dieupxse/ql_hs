using System.ComponentModel.DataAnnotations;

namespace QL_HS.Models
{
    public class UpdateStudentRequestModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Class { get; set; }
        [Required]
        public string Grade { get; set; }
        public string Bio { get; set; }
        public string Dob { get; set; }
    }
}
