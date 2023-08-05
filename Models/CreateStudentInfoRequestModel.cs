using System;
using System.ComponentModel.DataAnnotations;

namespace QL_HS.Models
{
    public class CreateStudentInfoRequestModel: CreateStudentRequestModel
    {
       
        [Required]
        public string GuardianName { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Password { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
    }
}
