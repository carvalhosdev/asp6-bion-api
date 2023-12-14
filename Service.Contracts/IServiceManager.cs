using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IServiceManager
    {
        //add service class here {get} - 60
        IAuthenticationService AuthenticationService { get; }
        IEmailSenderService EmailSenderService { get; }

    }
}
