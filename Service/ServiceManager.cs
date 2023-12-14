using AutoMapper;
using Contracts;
using Entities.ConfigurationModels;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ServiceManager : IServiceManager
    {
        //add your interface service here - readonly LAZY<I> - 61
        private readonly Lazy<IAuthenticationService> _authenticationService;
        private readonly Lazy<IEmailSenderService> _mailSenderService;

        public ServiceManager(
            IRepositoryManager repositoryManager, 
            ILoggerManager logger,
            IMapper mapper, 
            UserManager<User> userManager,
            IConfiguration configuration
            )
        {
            _authenticationService = new Lazy<IAuthenticationService>(() =>
            new AuthenticationService(logger, mapper, userManager, repositoryManager, configuration));

            _mailSenderService = new Lazy<IEmailSenderService>(() => 
            new EmailSenderService(logger, configuration));
        }

        public IAuthenticationService AuthenticationService => _authenticationService.Value;
        public IEmailSenderService EmailSenderService => _mailSenderService.Value;

    }
}
