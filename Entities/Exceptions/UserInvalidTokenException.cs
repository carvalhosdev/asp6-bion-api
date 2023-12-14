using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions
{
    public class UserInvalidTokenException: NotFoundException
    {
        public UserInvalidTokenException(string token)
            : base($"The token: {token} is invalid or not found")
        {
        }
    }
}
