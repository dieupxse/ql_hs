﻿using System.ComponentModel.DataAnnotations;
using System;

namespace QL_HS.Models
{
    public class CreateStudentRequestModel
    {
        [Required]
        public string Name { get; set; }
        public string Dob { get; set; }
        [Required]
        public string Class { get; set; }
        [Required]
        public string Grade { get; set; }
        public string Bio { get; set; }
        public int GuardianId { get; set; }
    }
}
