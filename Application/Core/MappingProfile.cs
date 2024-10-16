using Application.DTOs;
using AutoMapper;
using Domain.Entity;
using Domain.Enum;

namespace Application.Core
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Banner, BannerDto>();
            CreateMap<NewsBlog, NewsBlogDTO>();
            CreateMap<Banner, Banner>()
                .ForMember(b => b.Id, o => o.MapFrom(s => s.Id))
                .ForMember(b => b.Title, o => o.MapFrom(s => s.Title))
                .ForMember(b => b.ImageUrl, o => o.MapFrom(s => s.ImageUrl))
                .ForMember(b => b.LinkUrl, o => o.MapFrom(s => s.LinkUrl))
                .ForMember(b => b.StartDate, o => o.MapFrom(s => s.StartDate))
                .ForMember(b => b.EndDate, o => o.MapFrom(s => s.EndDate))
                .ForMember(b => b.CreatedAt, o => o.MapFrom(s => s.CreatedAt));
            CreateMap<Service, ServiceDto>()
             .ForMember(s => s.CourtClusterName, o => o.MapFrom(s => s.CourtCluster.CourtClusterName));
            CreateMap<Service, Service>();
            CreateMap<ServiceDto, Service>();
            CreateMap<CourtClustersInputDTO, CourtCluster>();
            CreateMap<OrderInputDTO, Order>();
            CreateMap<ReviewInputDTO, Review>();
            CreateMap<Review, Review>();
            CreateMap<ProductInputDTO, Product>();
            CreateMap<Product, Product>();
            CreateMap<Product, ProductDTO>()
            .ForMember(p => p.CategoryName, o => o.MapFrom(s => s.Category.CategoryName))
            .ForMember(p => p.CourtClusterName, o => o.MapFrom(s => s.CourtCluster.CourtClusterName));
            

            CreateMap<Booking, BookingDTO>()
            .ForMember(b => b.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(b => b.PaymentStatus, o => o.MapFrom(s => s.PaymentStatus.ToString()));
        }
    }
}