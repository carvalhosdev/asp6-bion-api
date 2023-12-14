using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<User> GetUserEmail(string email, bool trackChanges) =>
            await FindByCondition(e => e.Email.Equals(email), trackChanges)
            .SingleOrDefaultAsync();

        public async Task<User> GetUserResetToken(string resetToken, bool trackChanges) =>
            await FindByCondition(rt => rt.PasswordResetToken.Equals(resetToken), trackChanges)
            .SingleOrDefaultAsync();
        

        public async Task<User> GetUserVerificationToken(string token, bool trackChanges) =>
            await FindByCondition(t => t.VerificationToken.Equals(token), trackChanges)
            .SingleOrDefaultAsync();
       
    }
}
