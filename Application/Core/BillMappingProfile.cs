using Application.DTOs;
using AutoMapper;
using Domain.Entity;

namespace Application.Core
{
    /// <summary>
    ///  Mapping profile for bill
    /// </summary> 
    public class BillMappingProfile : Profile
    {
        public BillMappingProfile()
        {
            CreateMap<OrderDetail, ProductBillDto>()
                .ForMember(c => c.ProductName, o => o.MapFrom(s => s.Product.ProductName));

            CreateMap<OrderDetail, ServiceBillDto>()
                .ForMember(c => c.ServiceName, o => o.MapFrom(s => s.Service.ServiceName));
                
            CreateMap<CourtCluster, CourtClusterBillDto>();

            CreateMap<Booking, BookingBillDto>()
                .ForMember(c => c.CourtName, o => o.MapFrom(s => s.Court.CourtName));
        }
    }
}