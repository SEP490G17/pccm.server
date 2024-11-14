using Application.DTOs;
using AutoMapper;
using Domain;
using Domain.Entity;

namespace Application.Core
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Banner, BannerDto>();
            CreateMap<Banner, BannerInputDto>();
            CreateMap<BannerInputDto, Banner>();
            CreateMap<Banner, BannerLog>();
            CreateMap<Product, ProductLog>();
            CreateMap<NewsBlog, NewsBlogDto>();
            CreateMap<BookingInputDto, Booking>();

            CreateMap<Booking, BookingDtoV2>()
               .ForMember(dest => dest.CourtName, opt => opt.MapFrom(src => src.Court.CourtName))
               .ForMember(dest => dest.PlayTime,
                           opt => opt.MapFrom(src => $"{src.StartTime.AddHours(7):HH:mm} - {src.EndTime.AddHours(7):HH:mm}"))
               .ForMember(dest => dest.StartDay, opt => opt.MapFrom(src => src.StartTime))
               .ForMember(dest => dest.EndDay,
                           opt => opt.MapFrom(src => src.UntilTime != null ? src.UntilTime : src.EndTime))
               .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.Payment.Status))
               .ForMember(dest => dest.PaymentUrl, opt => opt.MapFrom(src => src.Payment.PaymentUrl));


            CreateMap<Booking, BookingDtoV2ForDetails>()
                         .ForMember(dest => dest.CourtName, opt => opt.MapFrom(src => src.Court.CourtName))
                         .ForMember(dest => dest.PlayTime,
                                     opt => opt.MapFrom(src => $"{src.StartTime.AddHours(7):HH:mm} - {src.EndTime.AddHours(7):HH:mm}"))
                         .ForMember(dest => dest.StartDay, opt => opt.MapFrom(src => src.StartTime))
                         .ForMember(dest => dest.EndDay,
                                     opt => opt.MapFrom(src => src.UntilTime != null ? src.UntilTime : src.EndTime))
                         .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.Payment.Status))
                         .ForMember(dest => dest.PaymentUrl, opt => opt.MapFrom(src => src.Payment.PaymentUrl))
                         .ForMember(dest => dest.CourtId, opt => opt.MapFrom(src => src.Court.Id))
                         .ForMember(dest => dest.CourtClusterId, opt => opt.MapFrom(src => src.Court.CourtClusterId))
                         .ForMember(dest => dest.Address, opt =>
                         opt.MapFrom(src => $"{src.Court.CourtCluster.Address}, {src.Court.CourtCluster.WardName}, {src.Court.CourtCluster.DistrictName}, {src.Court.CourtCluster.ProvinceName}"))
                         ;

            CreateMap<Order, OrderOfBookingDto>()
            .ForMember(o => o.PaymentStatus, src => src.MapFrom(s => s.Payment.Status));

            CreateMap<Booking, BookingDtoStatistic>()
            .ForMember(b => b.Id, o => o.MapFrom(s => s.Id))
                .ForMember(b => b.courtClusterName, o => o.MapFrom(s => s.Court.CourtCluster.CourtClusterName))
                .ForMember(b => b.ImageUrl, o => o.MapFrom(s => s.AppUser.ImageUrl))
                .ForMember(b => b.courtName, o => o.MapFrom(s => s.Court.CourtName))
                .ForMember(b => b.FullName, o => o.MapFrom(s => s.FullName));
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
            CreateMap<Service, Service>()
            .ForMember(s => s.UpdatedAt, o => o.MapFrom(s => (DateTime?)null))
                .ForMember(s => s.UpdatedBy, o => o.MapFrom(s => (string)null));
            CreateMap<ServiceInputDto, Service>()
              .ForMember(dest => dest.CourtClusterId,
               opt => opt.MapFrom(src => src.CourtClusterId != null && src.CourtClusterId.Any()
                                        ? (int?)src.CourtClusterId.First()
                                        : null));
            CreateMap<Service, ServiceInputDto>();
            CreateMap<ServiceDto, Service>();

            CreateMap<Court, CourtDto>()
            .ForMember(c => c.CourtId, o => o.MapFrom(s => s.Id));



            CreateMap<CourtClustersInputDto, CourtCluster>();

            CreateMap<CourtCluster, CourtClusterDto.CourtCLusterListPageUserSite>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.CourtClusterName))
            .ForMember(dest => dest.CourtClusterName, opt => opt.MapFrom(src => src.CourtClusterName))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Province, opt => opt.MapFrom(src => src.Province))
            .ForMember(dest => dest.ProvinceName, opt => opt.MapFrom(src => src.ProvinceName))
            .ForMember(dest => dest.District, opt => opt.MapFrom(src => src.District))
            .ForMember(dest => dest.DistrictName, opt => opt.MapFrom(src => src.DistrictName))
            .ForMember(dest => dest.Ward, opt => opt.MapFrom(src => src.Ward))
            .ForMember(dest => dest.WardName, opt => opt.MapFrom(src => src.WardName))
            .ForMember(dest => dest.NumbOfCourts, opt => opt.MapFrom(src => src.Courts.Count))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.OpenTime, opt => opt.MapFrom(src => src.OpenTime))
            .ForMember(dest => dest.CloseTime, opt => opt.MapFrom(src => src.CloseTime))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
            .ForMember(dest => dest.Services, opt => opt.MapFrom(src => src.Services.Select(s => new ServiceDto { ServiceName = s.ServiceName }).ToList()))
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products.Select(p => new ProductDto { ProductName = p.ProductName }).ToList()));

            CreateMap<CourtCluster, CourtClusterDto.CourtClusterListAll>();

            CreateMap<CourtCluster, CourtClusterDto.CourtCLusterListPage>()
                .ForMember(c => c.NumbOfCourts, o => o.MapFrom(st => st.Courts.Count()))
                .ForMember(c => c.Title, o => o.MapFrom(st => st.CourtClusterName));
            CreateMap<CourtCluster, CourtClusterDto.CourtCLusterDetails>()
                .ForMember(c => c.NumbOfCourts, o => o.MapFrom(st => st.Courts.Count()))
                .ForMember(c => c.Title, o => o.MapFrom(st => st.CourtClusterName));

            CreateMap<OrderInputDto, Order>();
            CreateMap<ReviewInputDto, Review>();
            CreateMap<Review, Review>();
            CreateMap<ProductInputDto, Product>()
            .ForMember(p => p.CategoryId, o => o.MapFrom(s => s.CategoryId))
            .ForMember(p => p.CourtClusterId, o => o.MapFrom(s => s.CourtClusterId))
            .ForMember(p => p.Quantity, o => o.MapFrom(st => st.Quantity))
            .ForMember(p => p.ProductName, o => o.MapFrom(st => st.ProductName))
            .ForMember(p => p.Description, o => o.MapFrom(st => st.Description))
            .ForMember(p => p.ThumbnailUrl, o => o.MapFrom(st => st.ThumbnailUrl))
            .ForMember(p => p.Price, o => o.MapFrom(st => st.Price))
            .ForMember(p => p.ImportFee, o => o.MapFrom(st => st.ImportFee))
            .ForMember(p => p.CreatedAt, o => o.Ignore());

            CreateMap<ProductDto, Product>();



            CreateMap<Product, ProductDto>()
            .ForMember(p => p.CategoryName, o => o.MapFrom(s => s.Category.CategoryName))
            .ForMember(p => p.CourtClusterName, o => o.MapFrom(s => s.CourtCluster.CourtClusterName))
            .ForMember(p => p.Quantity, o => o.MapFrom(st => st.Quantity))
            .ForMember(p => p.ImportFee, o => o.MapFrom(st => st.ImportFee))
            .ForMember(p => p.Price, o => o.MapFrom(st => st.Price));


            CreateMap<Product, ProductDto.ProductDetails>()
            .ForMember(p => p.ImportFee, o => o.MapFrom(st => st.ImportFee))
            .ForMember(p => p.Price, o => o.MapFrom(st => st.Price));

            CreateMap<Booking, BookingDto>()
            .ForMember(b => b.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(b => b.PaymentStatus, o => o.MapFrom(s => s.Payment.Status.ToString()))
            .ForMember(b => b.UserName, o => o.MapFrom(s => s.AppUser.UserName.ToString()))
            .ForMember(b => b.CourtName, o => o.MapFrom(s => s.Court.CourtName.ToString()));
            CreateMap<Booking, BookingDtoV1>()
                     .ForMember(b => b.Status, o => o.MapFrom(s => (int)s.Status))
                     .ForMember(b => b.PaymentStatus, o => o.MapFrom(s => (int)s.Payment.Status))
                     .ForMember(b => b.CourtName, o => o.MapFrom(s => s.Court.CourtName));

            CreateMap<StaffDetail, StaffDto>()
                .ForMember(st => st.FullName, o => o.MapFrom(s => $"{s.User.FirstName} {s.User.LastName}"))
                .ForMember(st => st.Position, o => o.MapFrom(s => s.Position.Name))
                .ForMember(st => st.PhoneNumber, o => o.MapFrom(st => st.User.PhoneNumber))
                .ForMember(st => st.CCCD, o => o.MapFrom(s => s.User.CitizenIdentification ?? "037202001234"))
                .ForMember(st => st.CourtCluster, o => o.MapFrom(s => s.StaffAssignments.Select(sa => sa.CourtCluster.CourtClusterName)));

            CreateMap<AppUser, UserDto>()
            .ForMember(u => u.FullName, o => o.MapFrom(au => $"{au.FirstName} {au.LastName}"));

            CreateMap<Review, ReviewDto>()
                .ForMember(st => st.FullName, o => o.MapFrom(s => $"{s.User.FirstName} {s.User.LastName}"));

            CreateMap<Expense, ExpenseDto>();
            CreateMap<Expense, ExpenseDetailDto>();
            CreateMap<ExpenseDto, Expense>();

            CreateMap<CourtPrice, PriceCourtDto>()
            .ForMember(c => c.CourtName, o => o.MapFrom(s => s.Court.CourtName))
            .ForMember(c => c.CourtId, o => o.MapFrom(s => s.Court.Id))
            .ForMember(c => c.Time, o => o.MapFrom(s => $"{s.FromTime.ToString("HH:mm")} - {s.ToTime.ToString("HH:mm")}"));
        }

    }
}