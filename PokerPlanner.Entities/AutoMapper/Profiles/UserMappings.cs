using AutoMapper;
using PokerPlanner.Entities.Domain.Mongo;
using PokerPlanner.Entities.DTO;

namespace PokerPlanner.Entities.AutoMapper.Profiles
{
    public class UserMappings : Profile, IMapperMarker
    {
        public UserMappings()
        {
            // Create bidirectional mapping from User -> UserDTO
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}