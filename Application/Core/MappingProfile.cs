using Application.DTOs;
using AutoMapper;
using Domain.Entity;

namespace Application.Core
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
             CreateMap<CourtClustersInputDTO, CourtCluster>();
        }
    }
}