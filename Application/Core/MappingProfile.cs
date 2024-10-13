using Application.DTOs;
using AutoMapper;
using Domain.Entity;

namespace Application.Core
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Banner, BannerDto>()
                .ForMember(b => b.Id, o => o.MapFrom(s => s.Id))
                .ForMember(b => b.Title, o => o.MapFrom(s => s.Title))
                .ForMember(b => b.ImageUrl, o => o.MapFrom(s => s.ImageUrl))
                .ForMember(b => b.LinkUrl, o => o.MapFrom(s => s.LinkUrl))
                .ForMember(b => b.StartDate, o => o.MapFrom(s => s.StartDate))
                .ForMember(b => b.EndDate, o => o.MapFrom(s => s.EndDate))
                .ForMember(b => b.CreatedAt, o => o.MapFrom(s => s.CreatedAt));
            CreateMap<Banner, Banner>()
                .ForMember(b => b.Id, o => o.MapFrom(s => s.Id))
                .ForMember(b => b.Title, o => o.MapFrom(s => s.Title))
                .ForMember(b => b.ImageUrl, o => o.MapFrom(s => s.ImageUrl))
                .ForMember(b => b.LinkUrl, o => o.MapFrom(s => s.LinkUrl))
                .ForMember(b => b.StartDate, o => o.MapFrom(s => s.StartDate))
                .ForMember(b => b.EndDate, o => o.MapFrom(s => s.EndDate))
                .ForMember(b => b.CreatedAt, o => o.MapFrom(s => s.CreatedAt));
             CreateMap<CourtClustersInputDTO, CourtCluster>();
             CreateMap<OrderInputDTO, Order>();
        }
    }
}