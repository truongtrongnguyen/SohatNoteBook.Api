using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SohatNoteBook.Authentication.Models.DTO.InComing
{
    public class TokenRequetsDto
    {
        [Required]
        public string Token { get; set; } = string.Empty;
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
