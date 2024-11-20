using Application.DTOs;
using AutoMapper;
using Domain.Entity;

namespace Application.Core
{
    public class CourtClusterMappingProfile : Profile
    {
        public CourtClusterMappingProfile()
        {
            // 1. Court Cluster
            #region Create CourtCluster
            CreateMap<CourtClustersInputDto, CourtCluster>()
                .ForMember(o => o.CourtClusterName, opt => opt.MapFrom(src => src.Title));

            CreateMap<CourtDetailsDto, Court>();
            CreateMap<CourtPricesDto, CourtPrice>();
            #endregion

            #region CourtCluster for user page
            CreateMap<CourtCluster, CourtClusterDto.CourtClusterListPageUserSite>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.CourtClusterName))
                .ForMember(dest => dest.NumbOfCourts, opt => opt.MapFrom(src => src.Courts.Count))
                .ForMember(dest => dest.Services, opt => opt.MapFrom(src =>
                    src.Services.Select(s => new ServiceDto { ServiceName = s.ServiceName }).ToList()))
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src =>
                    src.Products.Select(p => new ProductDto { ProductName = p.ProductName }).ToList()))
                .ForMember(dest => dest.Courts, opt => opt.MapFrom(src => src.Courts.Select(x => new CourtDto
                {
                    CourtId = x.Id,
                    CourtName = x.CourtName,
                }))
                );
            #endregion

            #region CourtCluster list all for elect options 
            CreateMap<CourtCluster, CourtClusterDto.CourtClusterListAll>();
            #endregion

            #region CourtCluster list all for admin page
            CreateMap<CourtCluster, CourtClusterDto.CourtCLusterListPage>()
                .ForMember(c => c.NumbOfCourts, o => o.MapFrom(st => st.Courts.Count()))
                .ForMember(c => c.Title, o => o.MapFrom(st => st.CourtClusterName));
            #endregion

            #region CourtCluster details
            CreateMap<CourtCluster, CourtClusterDto.CourtClusterDetails>()
                .ForMember(c => c.NumbOfCourts, o => o.MapFrom(st => st.Courts.Count()))
                .ForMember(c => c.Title, o => o.MapFrom(st => st.CourtClusterName))
                .ForMember(c => c.Address, o => o.MapFrom(src => $"{src.Address},{src.WardName},{src.DistrictName},{src.ProvinceName}"))
                .ForMember(c => c.MinPrice, o => o.MapFrom(src => src.Courts.SelectMany(court => court.CourtPrices).Min(price => price.Price)))
                .ForMember(c => c.MaxPrice, o => o.MapFrom(src => src.Courts.SelectMany(court => court.CourtPrices).Max(price => price.Price)));

            #endregion


            // 2. Court
            #region Court list all for options
            CreateMap<Court, CourtDto>()
                .ForMember(c => c.CourtId, o => o.MapFrom(s => s.Id));
            #endregion

            #region  Court for manager court page
            CreateMap<Court, CourtOfClusterDto>()
                .ForMember(dto => dto.CourtId, opt => opt.MapFrom(entity => entity.Id))
                .ForMember(dto => dto.CourtName, opt => opt.MapFrom(entity => entity.CourtName))
                .ForMember(dto => dto.MinPrice, opt =>
                {
                    opt.Condition(entity => entity.CourtPrices.Count() > 0);
                    opt.MapFrom(
                      entity => entity.CourtPrices.Min(price => price.Price)
                    );

                })
                .ForMember(dto => dto.MaxPrice, opt =>
                {
                    opt.Condition(entity => entity.CourtPrices.Count() > 0);
                    opt.MapFrom(
                    entity => entity.CourtPrices.Max(price => price.Price)
                    );
                })
                .ForMember(dto => dto.Status, opt => opt.MapFrom(entity => entity.Status))
                .ForMember(dto => dto.CourtPrices, opt => opt.MapFrom(entity => entity.CourtPrices));

            #endregion

            #region Court for unknow reason ?? 
            CreateMap<Court, CourtDetailsDto>();
            #endregion


            // 3. Court price
            #region  Court price for get list price
            CreateMap<CourtPrice, PriceCourtDto>()
                .ForMember(c => c.CourtName, o => o.MapFrom(s => s.Court.CourtName))
                .ForMember(c => c.CourtId, o => o.MapFrom(s => s.Court.Id))
                .ForMember(c => c.Time, o => o.MapFrom(s => $"{s.FromTime.ToString("HH:mm")} - {s.ToTime.ToString("HH:mm")}"));
            #endregion

            #region Court price of court response
            CreateMap<CourtPrice, CourtPriceResponseDto>();
            CreateMap<CourtPriceResponseDto, CourtPrice>();

            #endregion

        }
    }
}