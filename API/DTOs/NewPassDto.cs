using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class NewPassDto
    {
        public string CurrentPassword { get; set; }
        [StringLength(15, MinimumLength = 4)]
        public string NewPassword { get; set; }
    }
}