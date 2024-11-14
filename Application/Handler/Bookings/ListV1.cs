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
                var querySpec = request.BookingSpecParam;
                var spec = new BookingV1Specification(querySpec);
                var data = await _unitOfWork.Repository<Booking>()
                    .QueryList(spec)
                    .ProjectTo<BookingDtoV1>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);
                return Result<IReadOnlyList<BookingDtoV1>>.Success(data);
            }
        }
    }
}