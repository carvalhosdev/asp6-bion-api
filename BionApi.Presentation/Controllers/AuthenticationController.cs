using BionApi.Presentation.ActionFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BionApi.Presentation.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")] //334
    public class AuthenticationController : ControllerBase
    {
        private readonly IServiceManager _service;
        public AuthenticationController(IServiceManager service) => _service = service;

        [HttpPost("register")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]//actions filters 185
        public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistration)
        {
           var result = await _service.AuthenticationService.RegisterUser(userForRegistration);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }

                return BadRequest(ModelState);
            }

            //E-MAIL SENDER
            /*
            EmailSenderDto email = new EmailSenderDto
            {
                From = "email@example.com",
                To = userForRegistration.Email,
                Subject = "Welcome",
                Body = "<p>Message</p>"
            };

            await _service.EmailSenderService.SendSMTP(email);*/

            return StatusCode(201);
        }

        [HttpPost("login")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto user)
        {
            if(!await _service.AuthenticationService.ValidateUser(user))
            {
                return Unauthorized();
            }

            var tokenDto = await _service.AuthenticationService
                .CreateToken(populateExp: true);

            return Ok(tokenDto);
          
        }

        [HttpPost("verify-account")]
        public async Task<IActionResult> ActivateAccount(string token)
        {

            await _service.AuthenticationService
                .ValidateUserToken(token, trackChanges: true);
            //send email

            return Ok();

        }

        [HttpPost("forgot-password")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> ForgotPassword([FromBody] EmailDto request)
        {
            await _service.AuthenticationService
                .ForgotPassword(request.email, trackChanges: true);

            //send email

            return Ok();
        }

        [HttpPost("reset-password")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> ResetPassword([FromBody] UserPasswordResetDto request)
        {
            await _service.AuthenticationService
                .ResetPassword(request, trackChanges: true);

            return Ok();
        }

    }

}
