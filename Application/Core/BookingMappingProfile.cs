

using Application.DTOs;
using AutoMapper;
using Domain.Entity;

namespace Application.Core
{
    public class BookingMappingProfile : Profile
    {
        /// <summary>
        /// [Mapping Profile]
        /// Mapping profile for Booking and Order
        /// </summary>
        public BookingMappingProfile()
        {
            #region Booking for Create
            CreateMap<BookingInputDto, Booking>();
            #endregion

            #region Booking for admin list
            CreateMap<Booking, BookingDtoV2>()
                .ForMember(dest => dest.CourtClusterName, opt => opt.MapFrom(src => src.Court.CourtCluster.CourtClusterName))
                .ForMember(dest => dest.CourtName, opt => opt.MapFrom(src => src.Court.CourtName))
                .ForMember(dest => dest.PlayTime,
                            opt => opt.MapFrom(src => $"{src.StartTime.AddHours(7):HH:mm} - {src.EndTime.AddHours(7):HH:mm}"))
                .ForMember(dest => dest.StartDay, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.EndDay,
                            opt => opt.MapFrom(src => src.UntilTime != null ? src.UntilTime : src.EndTime))
                .ForMember(dest => dest.PaymentStatus, opt =>
                            {
                                opt.Condition(src => src.Payment != null);
                                opt.MapFrom(src => src.Payment.Status);
                            })
                .ForMember(dest => dest.PaymentUrl, opt => opt.MapFrom(src => src.Payment.PaymentUrl))
                .ForMember(dest => dest.CourtId, opt => opt.MapFrom(src => src.Court.Id))
                .ForMember(dest => dest.CourtClusterId, opt => opt.MapFrom(src => src.Court.CourtClusterId));
            #endregion

            #region Booking for schedule
            CreateMap<Booking, BookingDtoV1>()
                .ForMember(b => b.Status, o => o.MapFrom(s => (int)s.Status))
                .ForMember(dest => dest.PaymentStatus, opt =>
                            {
                                opt.Condition(src => src.Payment != null);
                                opt.MapFrom(src => src.Payment.Status);
                            })
                .ForMember(b => b.CourtName, o => o.MapFrom(s => s.Court.CourtName))
                .ForMember(b => b.CourtClusterId, o => o.MapFrom(src => src.Court.CourtClusterId));

            #endregion

            #region Booking for user history
            CreateMap<Booking, BookingUserHistoryDto>()
                .ForMember(dest => dest.Address, opt =>
                            opt.MapFrom(src => $"{src.Court.CourtCluster.DistrictName}, {src.Court.CourtCluster.ProvinceName}"))
                .ForMember(dest => dest.TimePlay,
                            opt => opt.MapFrom(src => $"{src.StartTime.AddHours(7):HH:mm} - {src.EndTime.AddHours(7):HH:mm}"))
                .ForMember(dest => dest.EndTime,
                            opt => opt.MapFrom(src => src.UntilTime != null ? src.UntilTime : src.EndTime))
                .ForMember(dest => dest.PaymentStatus, opt =>
                       {
                           opt.Condition(src => src.Payment != null);
                           opt.MapFrom(src => src.Payment.Status);
                       })
                .ForMember(dest => dest.CourtClusterName, opt => opt.MapFrom(src => src.Court.CourtCluster.CourtClusterName))
                .ForMember(dest => dest.CourtClusterId, opt => opt.MapFrom(src => src.Court.CourtClusterId))
                .ForMember(dest => dest.CourtName, opt => opt.MapFrom(src => src.Court.CourtName));
            #endregion

            #region Booking Details and Order
            //1. Booking
            CreateMap<Booking, BookingDtoV2ForDetails>()
                           .ForMember(dest => dest.CourtName, opt => opt.MapFrom(src => src.Court.CourtName))
                           .ForMember(dest => dest.PlayTime,
                                                opt => opt.MapFrom(src => $"{src.StartTime.AddHours(7):HH:mm} - {src.EndTime.AddHours(7):HH:mm}"))
                           .ForMember(dest => dest.StartDay, opt => opt.MapFrom(src => src.StartTime))
                           .ForMember(dest => dest.EndDay,
                                                opt => opt.MapFrom(src => src.UntilTime != null ? src.UntilTime : src.EndTime))
                           .ForMember(dest => dest.PaymentStatus, opt =>
                                   {
                                       opt.Condition(src => src.Payment != null);
                                       opt.MapFrom(src => src.Payment.Status);
                                   })
                                           .ForMember(dest => dest.PaymentUrl, opt => opt.MapFrom(src => src.Payment.PaymentUrl))
                           .ForMember(dest => dest.CourtId, opt => opt.MapFrom(src => src.Court.Id))
                           .ForMember(dest => dest.CourtClusterId, opt => opt.MapFrom(src => src.Court.CourtClusterId))
                           .ForMember(dest => dest.Address, opt =>
                                    opt.MapFrom(src => $"{src.Court.CourtCluster.Address}, {src.Court.CourtCluster.WardName}, {src.Court.CourtCluster.DistrictName}, {src.Court.CourtCluster.ProvinceName}"));

            //2. Order
            CreateMap<Order, OrderDetailsResponse>()
                .ForMember(dest => dest.PaymentStatus, opt =>
                {
                    opt.Condition(src => src.Payment != null);
                    opt.MapFrom(src => src.Payment.Status);
                })
                .ForMember(dest => dest.OrderForProducts, opt => opt.MapFrom(src =>
                    src.OrderDetails
                        .Where(od => od.ProductId != null)
                        .Select(od => new ProductsForOrderDetailsResponse
                        {
                            ProductId = od.ProductId.Value,
                            Quantity = od.Quantity,
                            ProductName = od.Product.ProductName,
                            Price = od.Price,
                            CurrPrice = od.Product.Price,
                            TotalPrice = od.Price * (decimal)od.Quantity,
                            CurrTotalPrice = od.Product.Price * (decimal)od.Quantity
                        })
                ))
                .ForMember(dest => dest.OrderForServices, opt => opt.MapFrom(src =>
                    src.OrderDetails
                        .Where(od => od.ServiceId != null)
                        .Select(od => new ServicesForOrderDetailsResponse
                        {
                            ServiceId = od.ServiceId.Value,
                            ServiceName = od.Service.ServiceName,
                            Price = od.Price,
                            CurrPrice = od.Service.Price,
                            TotalPrice = od.Price * (decimal)od.Quantity,
                            CurrTotalPrice = od.Service.Price * (decimal)od.Quantity
                        })
                ));


            CreateMap<Order, OrderOfBookingDto>()
                .ForMember(dest => dest.PaymentStatus, opt =>
                    {
                        opt.Condition(src => src.Payment != null);
                        opt.MapFrom(src => src.Payment.Status);
                    });
            #endregion

            #region Order for Create
            CreateMap<OrderInputDto, Order>();
            #endregion

            #region Booking for unkown doing ???
            CreateMap<Booking, BookingDto>()
                    .ForMember(b => b.Status, o => o.MapFrom(s => s.Status.ToString()))
                    .ForMember(dest => dest.PaymentStatus, opt =>
                        {
                            opt.Condition(src => src.Payment != null);
                            opt.MapFrom(src => src.Payment.Status.ToString());
                        }).ForMember(b => b.UserName, o => o.MapFrom(s => s.AppUser.UserName.ToString()))

                    .ForMember(b => b.CourtName, o => o.MapFrom(s => s.Court.CourtName.ToString()));
            #endregion
        }
    }
}