using AutoMapper;
using Contracts;
using Entities.ConfigurationModels;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Service.Contracts;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public sealed class AuthenticationService : IAuthenticationService
    {
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IRepositoryManager _repositoryManager;

        private readonly JwtConfiguration _jwtConfiguration;
        private User? _user;

        public AuthenticationService(
            ILoggerManager logger, 
            IMapper mapper,
            UserManager<User> userManager,
            IRepositoryManager repositoryManager,
            IConfiguration configuration
            )
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _repositoryManager = repositoryManager;
            _configuration = configuration;
            _jwtConfiguration = new JwtConfiguration();
            _configuration.Bind(_jwtConfiguration.Section, _jwtConfiguration);
        }
     
        /**
         * REGISTER METHOD
         */
        public async Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistrationDto)
        {
            var user = _mapper.Map<User>(userForRegistrationDto);
            user.VerificationToken = CreateRandomToken();
            var result = await _userManager.CreateAsync(user, userForRegistrationDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, userForRegistrationDto.Roles);
                //send email
            }

            //check 296 RoleManager
            return result;
        }
      
        /**
         *  LOGIN METHOD
         */
        public async Task<bool> ValidateUser(UserForAuthenticationDto userForAuth)
        {
            _user = await _userManager.FindByNameAsync(userForAuth.Username);

            var result = (
                _user != null && 
                await _userManager.CheckPasswordAsync(_user, userForAuth.Password)
                && _user.VerifiedAt != null
                );

            if (!result)
            {
                _logger.LogWarn($"{nameof(ValidateUser)}: Authentication failed. Wrong user name or password or Not Verified");
            }

            return result;
        }

        /**
         *  GENERATE TOKEN METHOD
         */
        public async Task<TokenDto> CreateToken(bool populateExp)
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims();
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

            var refreshToken = GenerateRefreshToken();
            _user.RefreshToken = refreshToken;

            if (populateExp)
            {
                _user.ResfreshTokenExpiryTime = DateTime.Now.AddDays(7);
            }

            await _userManager.UpdateAsync(_user);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return new TokenDto(accessToken, refreshToken);

        }
   
        /**
         *  REFRESH TOKEN METHOD
         */
        public async Task<TokenDto> RefreshToken(TokenDto tokenDto)
        {
            var principal = GetPrincipalFromExpiredToken(tokenDto.AccesToken);
            var user = await _userManager.FindByNameAsync(principal.Identity.Name);
            if(user == null || user.RefreshToken != tokenDto.RefreshToken ||
                user.ResfreshTokenExpiryTime <= DateTime.Now
                )
            {
                throw new RefreshTokenBadRequest();
            }

            _user = user;

            return await CreateToken(populateExp: false);
        }

        /**
         *  GET USER INFORMATION ABOUT BY TOKEN
         */
        public async Task<UserDto> GetUserVerificationToken(string token, bool trackChanges)
        {
            var user = await _repositoryManager.User.GetUserVerificationToken(token, trackChanges);

            if(user is null)
            {
                throw new UserInvalidTokenException(token);
            }
            var userDto = _mapper.Map<UserDto>(user);
            return userDto;
        }

        /*
         *  GET USER INFORMATION ABOUT BY EMAIL
         */
        public async Task<UserDto> GetUserEmail(string email, bool trackChanges)
        {
            var user = await _repositoryManager.User.GetUserEmail(email, trackChanges);
            if(user is null)
            {
                throw new UserNotFoundException();
            }
            var userDto = _mapper.Map<UserDto>(user);
            return userDto;
        }

        /*
         * ACTIVATE ACCOUNT METHOD
         */
        public async Task ValidateUserToken(string token, bool trackChanges)
        {
            var user = await _repositoryManager.User.GetUserVerificationToken(token, trackChanges);
            if(user is null)
            {
                throw new UserInvalidTokenException(token);
            }

            user.VerifiedAt = DateTime.Now;
            user.EmailConfirmed = true;
            await _repositoryManager.Save();
        }

        /**
         *  FORGOT PASSWORD METHOD
         */
        public async Task ForgotPassword(string email, bool trackChanges)
        {
            var user = await _repositoryManager.User.GetUserEmail(email, trackChanges);
            if(user is null)
            {
                throw new UserNotFoundException();
            }

            //storing the token to send to user and use in resetPassword the token
            user.PasswordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            user.ResetTokenExpires = DateTime.Now.AddDays(1);
            await _repositoryManager.Save();
        }

        /**
         *  GET RESET TOKEN METOHD
         */
        public async Task<UserDto> GetUserResetToken(string resetToken, bool trackChanges)
        {
            var user = await _repositoryManager.User.GetUserResetToken(resetToken, trackChanges);
            if(user is null)
            {
                throw new UserInvalidTokenException(resetToken);
            }

            var userDto = _mapper.Map<UserDto>(user);
            return userDto;
        }

        /**
         *  RESET PASSWORD METHOD
         * 
         */
        public async Task ResetPassword(UserPasswordResetDto userPasswordResetDto, bool trackChanges)
        {
            var user = await _repositoryManager.User
                .GetUserResetToken(userPasswordResetDto.ResetToken, trackChanges);

            if(user is null || user.ResetTokenExpires < DateTime.Now)
            {
                throw new UserInvalidTokenException(userPasswordResetDto.ResetToken);
            }

            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;
            var newPasswordSaved = await _userManager
                .ResetPasswordAsync(user, userPasswordResetDto.ResetToken, userPasswordResetDto.Password);

            if (newPasswordSaved.Errors.Count() > 0)
            {
                foreach (var error in newPasswordSaved.Errors)
                {
                    throw new MessageException(error.Description);
                }
            }
        }


        //-----------------------------------------------------------------------------------------------------------------


        /**
         * PRIVATE METHODS 
         */
        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET"));
            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, _user.UserName),
                new Claim(ClaimTypes.PrimarySid, _user.Id)
            };

            var roles = await _userManager.GetRolesAsync(_user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var tokenOptions = new JwtSecurityToken(
                issuer: _jwtConfiguration.ValidIssuer,
                audience: _jwtConfiguration.ValidAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtConfiguration.Expires)),
                signingCredentials: signingCredentials
                );

            return tokenOptions;

        }

        ///Impementig the refreshToken - 316
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        //307
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var jwtSettings = _configuration.GetSection("jwtSettings");
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET"))),
                ValidateLifetime = true,
                ValidIssuer = _jwtConfiguration.ValidIssuer,
                ValidAudience = _jwtConfiguration.ValidAudience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken); ;
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }

}
