using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
    public record UserForRegistrationDto
    {
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; init; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string PhoneNumber { get; init; } = string.Empty;
        public ICollection<string>? Roles { get; init; }

        //294
    }
}
