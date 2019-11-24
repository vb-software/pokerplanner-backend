using AutoMapper;
using PokerPlanner.Entities.Domain.Mongo;
using PokerPlanner.Entities.DTO;

namespace PokerPlanner.Entities.AutoMapper.Profiles
{
    public class WorkspaceMappings : Profile, IMapperMarker
    {
        public WorkspaceMappings()
        {
            CreateMap<CreateWorkspaceDto, Workspace>();
            CreateMap<CreateWorkspaceReleaseDto, Release>();
        }
    }
}