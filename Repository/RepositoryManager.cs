using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _repositoryContext;

        //extension
        private readonly Lazy<IUserRepository> _userRepository;


        public RepositoryManager(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
            _userRepository =new Lazy<IUserRepository>(() => new UserRepository(repositoryContext));
            //PG 58
        }

        public IUserRepository User => _userRepository.Value;
        public async Task Save() => await _repositoryContext.SaveChangesAsync();
       
    }
}
