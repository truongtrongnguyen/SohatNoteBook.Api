using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SohatNoteBook.Authentication.Models.DTO.InComing
{
    public class UserRegistrantionRequestDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        [EmailAddress(ErrorMessage = "Email Invalid")]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Passwork { get; set; } = string.Empty;
    }
}
