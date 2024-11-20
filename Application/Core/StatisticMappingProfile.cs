

using Application.DTOs;
using AutoMapper;
using Domain.Entity;

namespace Application.Core
{
    /// <summary>
    /// Mapping profile for statistic
    /// </summary>
    public class StatisticMappingProfile:Profile
    {
        public StatisticMappingProfile()
        {
            CreateMap<Booking, BookingDtoStatistic>()
                .ForMember(b => b.Id, o => o.MapFrom(s => s.Id))
                .ForMember(b => b.courtClusterName, o => o.MapFrom(s => s.Court.CourtCluster.CourtClusterName))
                .ForMember(b => b.ImageUrl, o => o.MapFrom(s => s.AppUser.ImageUrl))
                .ForMember(b => b.courtName, o => o.MapFrom(s => s.Court.CourtName))
                .ForMember(b => b.FullName, o => o.MapFrom(s => s.FullName));
        }
    }
}