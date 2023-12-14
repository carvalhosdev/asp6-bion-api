using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IRepositoryManager
    {
        //add your repositories models here {get} - 57
        IUserRepository User { get; }
        Task Save();
    }
}
