using System;
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
            CreateMap<CreateWorkspaceReleaseDto, Release>()
                .ForMember(dest => dest.Guid, opt => opt.Ignore());
            CreateMap<CreateWorkspaceReleaseIterationDto, Iteration>()
                .ForMember(dest => dest.Guid, opt => opt.Ignore());

            CreateMap<Workspace, WorkspaceSummaryDto>()
                .ForMember(
                    dest => dest.ScoreSystem,
                    opt => {
                        opt.PreCondition(src => src.Configuration != null);
                        opt.MapFrom(src => src.Configuration.ScoreSystem);
                    }
                )
                .ForMember(
                    dest => dest.HideUserVotes,
                    opt =>
                    {
                        opt.PreCondition(src => src.Configuration != null);
                        opt.MapFrom(src => src.Configuration.HideUserVotes);
                    }
                )
                .ForMember(
                    dest => dest.AllowRevotes,
                    opt =>
                    {
                        opt.PreCondition(src => src.Configuration != null);
                        opt.MapFrom(src => src.Configuration.AllowRevotes);
                    }
                )
                .ForMember(
                    dest => dest.ReleasesCount,
                    opt =>
                    {
                        opt.PreCondition(src => src.Releases != null);
                        opt.MapFrom(src => src.Releases.Count);
                    }
                )
                .ForMember(
                    dest => dest.UsersCount,
                    opt =>
                    {
                        opt.PreCondition(src => src.Users != null);
                        opt.MapFrom(src => src.Users.Count);
                    }
                );
        }
    }
}