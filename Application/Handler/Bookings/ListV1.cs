using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Application.SpecParams.BookingSpecification;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Extensions;

namespace Application.Handler.Bookings
{
    public class ListV1
    {
        public class Query : IRequest<Result<IReadOnlyList<BookingDtoV1>>>
        {
            public BookingV1SpecParam BookingSpecParam { get; set; }
        }

        public class Handler(IMapper _mapper, IUnitOfWork _unitOfWork) : IRequestHandler<Query, Result<IReadOnlyList<BookingDtoV1>>>
        {
            public async Task<Result<IReadOnlyList<BookingDtoV1>>> Handle(Query request, CancellationToken cancellationToken)
            {
                DateTime selectedDate = DateTime.Today;
                if(request.BookingSpecParam.SelectedDate != null){
                    selectedDate = request.BookingSpecParam.SelectedDate.Value;
                }
                
                var fromDate = selectedDate.StartOfWeek(DayOfWeek.Sunday).ToUniversalTime();
                var toDate = selectedDate.EndOfWeek(DayOfWeek.Sunday).ToUniversalTime();
                var bookingStatus = request.BookingSpecParam.BookingStatus;
                var courtClusterId = request.BookingSpecParam.CourtClusterId;
                var spec = new BookingV1Specification(fromDate,toDate,bookingStatus,courtClusterId);

                var data = await _unitOfWork.Repository<Booking>()
                    .QueryList(spec)
                    .ProjectTo<BookingDtoV1>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);
                return Result<IReadOnlyList<BookingDtoV1>>.Success(data);
            }
        }



    }



}