using Microsoft.AspNetCore.Identity;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IAuthenticationService
    {
        Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistrationDto);
        Task<bool> ValidateUser(UserForAuthenticationDto userForAuth);
        Task<UserDto> GetUserVerificationToken(string token, bool trackChanges);
        Task<UserDto> GetUserEmail(string email, bool trackChanges);
        Task<UserDto> GetUserResetToken(string resetToken, bool trackChanges);
        Task ValidateUserToken(string token, bool trackChanges);
        Task ForgotPassword(string email, bool trackChanges);
        Task ResetPassword(UserPasswordResetDto userPasswordResetDto, bool trackChanges);
        Task<TokenDto> CreateToken(bool populateExp);
        Task<TokenDto> RefreshToken(TokenDto tokenDto);

    }
}
