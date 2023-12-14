using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IUserRepository
    {
        Task<User> GetUserVerificationToken(string token, bool trackChanges);
        Task<User> GetUserEmail(string email, bool trackChanges);
        Task<User> GetUserResetToken(string resetToken, bool trackChanges);


    }
}
