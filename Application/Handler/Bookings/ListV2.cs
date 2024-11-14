using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Application.SpecParams.BookingSpecification;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handler.Bookings
{
    public class ListV2
    {
        public class Query : IRequest<Result<Pagination<BookingDtoV2>>>
        {
            public BookingSpecParam BookingSpecParam { get; set; }
        }

        public class Handler(IMapper _mapper, IUnitOfWork _unitOfWork) : IRequestHandler<Query, Result<Pagination<BookingDtoV2>>>
        {
            public async Task<Result<Pagination<BookingDtoV2>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var querySpec = request.BookingSpecParam;
                var spec = new BookingV2Specification(querySpec);
                var specCount = new BookingV2CourtSpecification(querySpec);
                var data = await _unitOfWork.Repository<Booking>()
                    .QueryList(spec)
                    .ProjectTo<BookingDtoV2>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);
                var total = await _unitOfWork.Repository<Booking>().CountAsync(specCount, cancellationToken);

                var response = new Pagination<BookingDtoV2>()
                {
                    Count = total,
                    Data = data,
                    PageSize = request.BookingSpecParam.PageSize

                };
                return Result<Pagination<BookingDtoV2>>.Success(response);
            }
        }
    }
}