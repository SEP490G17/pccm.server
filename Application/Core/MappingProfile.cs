using Application.DTOs;
using AutoMapper;
using Domain;
using Domain.Entity;
using Domain.Enum;
using Microsoft.AspNetCore.Identity;

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

            CreateMap<CourtCluster, CourtClustersInputDTO>()
             .ForMember(b => b.Id, o => o.MapFrom(s => s.Id))
                .ForMember(b => b.CourtClusterName, o => o.MapFrom(s => s.CourtClusterName))
                .ForMember(b => b.Location, o => o.MapFrom(s => s.Location))
                .ForMember(b => b.Address, o => o.MapFrom(s => s.Address))
                .ForMember(b => b.OwnerId, o => o.MapFrom(s => s.OwnerId))
                .ForMember(b => b.Description, o => o.MapFrom(s => s.Description))
                .ForMember(b => b.Images, o => o.MapFrom(s => s.Images))
                .ForMember(b => b.CreatedAt, o => o.MapFrom(s => s.CreatedAt));
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
            .ForMember(b => b.PaymentStatus, o => o.MapFrom(s => s.PaymentStatus.ToString()))
             .ForMember(b => b.UserName, o => o.MapFrom(s => s.User.UserName.ToString()))
            .ForMember(b => b.CourtName, o => o.MapFrom(s => s.Court.CourtName.ToString()));


        CreateMap<StaffDetail, StaffDto>()
            .ForMember(st => st.FullName, o => o.MapFrom(s => $"{s.User.FirstName} {s.User.LastName}"))
            .ForMember(st => st.Position, o => o.MapFrom(s => s.Position.Name))
            .ForMember(st => st.PhoneNumber, o => o.MapFrom(st => st.User.PhoneNumber))
            .ForMember(st => st.CCCD, o => o.MapFrom(s => s.User.CitizenIdentification ?? "037202001234"))
            .ForMember(st => st.CourtCluster, o => o.MapFrom(s => s.StaffAssignments.Select(sa => sa.CourtCluster.CourtClusterName)));

            CreateMap<AppUser, UserDto>()
            .ForMember(u => u.FullName, o => o.MapFrom(au => $"{au.FirstName} {au.LastName}"));
        }

}
}