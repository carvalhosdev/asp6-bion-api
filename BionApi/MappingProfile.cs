using AutoMapper;
using Entities.Models;
using Shared.DataTransferObjects;

namespace BionApi
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            //add your maps here 78
            CreateMap<User, UserDto>();
            CreateMap<UserForRegistrationDto, User>();
        }
    }
}
