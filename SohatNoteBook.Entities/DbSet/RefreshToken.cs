using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SohatNoteBook.Entities.DbSet
{
    public class RefreshToken : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;      // User Id when logged in
        public string Token { get; set; } = string.Empty;
        public string JwtId { get; set; } = string.Empty;       // the id generate when a jwt id has been requested
        public bool IsUsed { get; set; }                        // To make sure that the token is only used once
        public bool IsRevoked { get; set; }                     // make sure they are valid
        public DateTime ExpiryTime { get; set; }
        [ForeignKey(nameof(UserId))]
        public IdentityUser User {get;set;}
    }
}
