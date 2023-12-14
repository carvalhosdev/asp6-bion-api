using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects
{
    public record UserPasswordResetDto
    {
        [Required]
        public string ResetToken { get; set; } = string.Empty;

        [Required, MinLength(10, ErrorMessage = "Please, enter at least 10 characters")]
        public string Password { get; set; } = string.Empty;

        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

    }
}
