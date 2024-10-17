using Application.DTOs;
using AutoMapper;
using Domain.Entity;
using System.Runtime.ConstrainedExecution;

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
            CreateMap<Service, ServiceDto>();
            CreateMap<Service, Service>();
            CreateMap<ServiceDto, Service>();
            CreateMap<CourtClustersInputDTO, CourtCluster>();
            CreateMap<OrderInputDTO, Order>();
            CreateMap<ReviewInputDTO, Review>();

            CreateMap<Booking, BookingDTO>()
            .ForMember(b => b.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(b => b.PaymentStatus, o => o.MapFrom(s => s.PaymentStatus.ToString()));
        }
    }
}