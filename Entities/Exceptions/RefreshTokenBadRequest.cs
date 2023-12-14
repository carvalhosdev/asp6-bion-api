using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions
{
    public sealed class RefreshTokenBadRequest: BadRequestException
    {
        //322
        public RefreshTokenBadRequest()
            :base("Invalid client request. The tokenDRO has some invalid values.")
        {
        }

    }
}
