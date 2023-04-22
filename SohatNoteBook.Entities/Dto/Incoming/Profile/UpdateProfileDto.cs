using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SohatNoteBook.Entities.Dto.Incoming.Profile
{
    public class UpdateProfileDto
    {
        public string Country { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string Sex { get; set; } = string.Empty;
    }
}
