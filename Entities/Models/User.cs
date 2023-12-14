using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class User: IdentityUser
    {
        ///290
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string VerificationToken { get; set; } = string.Empty;

        ///user recovery
        public DateTime? VerifiedAt { get; set; }

        //todo
        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
        //todo

        //refreshToken 315
        public string? RefreshToken { get; set; } = string.Empty;
        public DateTime ResfreshTokenExpiryTime { get; set; }
    }
}
