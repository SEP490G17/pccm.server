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
            #region Product Profile
            CreateMap<ProductDto, Product>();

            CreateMap<Product, ProductDto>()
                .ForMember(p => p.CategoryName, o => o.MapFrom(s => s.Category.CategoryName))
                .ForMember(p => p.CourtClusterName, o => o.MapFrom(s => s.CourtCluster.CourtClusterName))
                .ForMember(p => p.Quantity, o => o.MapFrom(st => st.Quantity))
                .ForMember(p => p.ImportFee, o => o.MapFrom(st => st.ImportFee))
                .ForMember(p => p.Price, o => o.MapFrom(st => st.Price));

            CreateMap<Product, ProductDto.ProductDetails>()
                .ForMember(p => p.ImportFee, o => o.MapFrom(st => st.ImportFee))
                .ForMember(p => p.Price, o => o.MapFrom(st => st.Price))
                .ForMember(p => p.CourtClusterName, o => o.MapFrom(st => st.CourtCluster.CourtClusterName));

            CreateMap<Product, ProductLog>();

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

            CreateMap<ProductLog, ProductLogDto>()
              .ForMember(c => c.CategoryName, o => o.MapFrom(s => s.Category.CategoryName))
              .ForMember(c => c.CreateBy, o => o.MapFrom(s => s.CreatedBy))
              .ForMember(c => c.CreateAt, o => o.MapFrom(s => s.CreatedAt))
              .ForMember(c => c.LogType, o => o.MapFrom(s => s.LogType.ToString()))
              .ForMember(c => c.CourtClusterName, o => o.MapFrom(s => s.CourtCluster.CourtClusterName));

            CreateMap<Product, ProductLog>();
            CreateMap<Product, ProductLog>()
              .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
              .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
              .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id));


            CreateMap<ProductLog, ProductLogDto>()
                       .ForMember(c => c.CategoryName, o => o.MapFrom(s => s.Category.CategoryName))
                       .ForMember(c => c.CreateBy, o => o.MapFrom(s => s.CreatedBy))
                       .ForMember(c => c.CreateAt, o => o.MapFrom(s => s.CreatedAt))
                       .ForMember(c => c.LogType, o => o.MapFrom(s => s.LogType.ToString()))
                       .ForMember(c => c.CourtClusterName, o => o.MapFrom(s => s.CourtCluster.CourtClusterName));


            #endregion

            #region Banner Profile
            CreateMap<Banner, BannerDto>();

            CreateMap<Banner, BannerInputDto>();

            CreateMap<BannerInputDto, Banner>();

            CreateMap<Banner, BannerLog>();

            CreateMap<Banner, Banner>()
                .ForMember(b => b.Id, o => o.MapFrom(s => s.Id))
                .ForMember(b => b.Title, o => o.MapFrom(s => s.Title))
                .ForMember(b => b.ImageUrl, o => o.MapFrom(s => s.ImageUrl))
                .ForMember(b => b.LinkUrl, o => o.MapFrom(s => s.LinkUrl))
                .ForMember(b => b.StartDate, o => o.MapFrom(s => s.StartDate))
                .ForMember(b => b.EndDate, o => o.MapFrom(s => s.EndDate))
                .ForMember(b => b.CreatedAt, o => o.MapFrom(s => s.CreatedAt));
            #endregion

            #region News Profile
            CreateMap<NewsBlog, NewsBlogDto>();
            #endregion

            #region User Profile
            CreateMap<AppUser, ProfileInputDto>();

            CreateMap<ProfileInputDto, AppUser>();

            CreateMap<AppUser, ProfileDto>();
            
            CreateMap<AppUser, UserDto>()
                .ForMember(u => u.FullName, o => o.MapFrom(au => $"{au.FirstName} {au.LastName}"));
            #endregion

            #region Service Profile
            CreateMap<Service, ServiceDto>()
                .ForMember(s => s.CourtClusterName, o => o.MapFrom(s => s.CourtCluster.CourtClusterName));

            CreateMap<Service, Service>()
                .ForMember(s => s.UpdatedAt, o => o.MapFrom(s => (DateTime?)null))
                .ForMember(s => s.UpdatedBy, o => o.MapFrom(s => (string)null));

            CreateMap<ServiceInputDto, Service>()
              .ForMember(dest => dest.CourtClusterId,
               opt => opt.MapFrom(src =>
                        src.CourtClusterId != null
                        && src.CourtClusterId.Any() ? (int?)src.CourtClusterId.First() : null));

            CreateMap<Service, ServiceInputDto>();

            CreateMap<ServiceDto, Service>();
            CreateMap<Service, ServiceLog>()
              .ForMember(c => c.ServiceId, o => o.MapFrom(s => s.Id))
              .ForMember(c => c.ServiceName, o => o.MapFrom(s => s.ServiceName))
              .ForMember(c => c.Price, o => o.MapFrom(s => s.Price));


            CreateMap<ServiceLog, ServiceLogDto>()
              .ForMember(c => c.LogType, o => o.MapFrom(s => s.LogType.ToString()))
              .ForMember(c => c.CreateBy, o => o.MapFrom(s => s.CreatedBy))
              .ForMember(c => c.CreateAt, o => o.MapFrom(s => s.CreatedAt))
              .ForMember(c => c.CourtClusterName, o => o.MapFrom(s => s.CourtCluster.CourtClusterName));

            #endregion

            #region Review Profile
            CreateMap<ReviewInputDto, Review>();

            CreateMap<Review, Review>();

            CreateMap<Review, ReviewDto>()
                .ForMember(st => st.PhoneNumber, o => o.MapFrom(s => s.User.PhoneNumber))
                .ForMember(st => st.FullName, o => o.MapFrom(s => $"{s.User.FirstName} {s.User.LastName}"));
            #endregion

            #region Expense Profile
            CreateMap<Expense, ExpenseDto>();
            CreateMap<Expense, ExpenseDetailDto>();
            CreateMap<ExpenseDto, Expense>();
            #endregion

            CreateMap<StaffDetail, StaffDto>()
                .ForMember(st => st.FullName, o => o.MapFrom(s => $"{s.User.FirstName} {s.User.LastName}"))
                .ForMember(st => st.Email, o => o.MapFrom(s => s.User.Email))
                .ForMember(st => st.Position, o => o.MapFrom(s => s.Position.Name))
                .ForMember(st => st.Roles, o => o.MapFrom(s => s.Position.DefaultRoles))
                .ForMember(st => st.PhoneNumber, o => o.MapFrom(st => st.User.PhoneNumber))
                .ForMember(st => st.CourtCluster, o => o.MapFrom(s => s.StaffAssignments.Select(sa => sa.CourtCluster.CourtClusterName)));

            CreateMap<StaffDetail, StaffDetailDto>()
                .ForMember(st => st.FirstName, o => o.MapFrom(s => s.User.FirstName))
                .ForMember(st => st.LastName, o => o.MapFrom(s => s.User.LastName))
                .ForMember(st => st.Email, o => o.MapFrom(s => s.User.Email))
                .ForMember(st => st.Position, o => o.MapFrom(s => s.Position.Id))
                .ForMember(st => st.PhoneNumber, o => o.MapFrom(st => st.User.PhoneNumber))
                .ForMember(st => st.UserName, o => o.MapFrom(st => st.User.UserName))
                .ForMember(st => st.CourtCluster, o => o.MapFrom(s => s.StaffAssignments.Select(sa => sa.CourtCluster.Id)));
        }

    }
}