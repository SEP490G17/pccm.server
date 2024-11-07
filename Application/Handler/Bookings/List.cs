using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Application.SpecParams;
using Application.SpecParams.BookingCountSpecification;
using Application.SpecParams.BookingSpecification;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handler.Bookings
{
    public class List
    {
        public class Query : IRequest<Result<Pagination<BookingDto>>>
        {
            public BaseSpecWithFilterParam BaseSpecWithFilterParam { get; set; }
        }

        public class Handler(IMapper _mapper, IUnitOfWork _unitOfWork) : IRequestHandler<Query, Result<Pagination<BookingDto>>>
        {
            public async Task<Result<Pagination<BookingDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var querySpec = request.BaseSpecWithFilterParam;

                var spec = new BookingSpecification(querySpec);
                var specCount = new BookingCountSpecification(querySpec);

                var totalElement = await _unitOfWork.Repository<Booking>().CountAsync(specCount, cancellationToken);
               
                var data = await _unitOfWork.Repository<Booking>()
                    .QueryList(spec) 
                    .Include(a => a.Court) 
                    .Include(a=>a.AppUser)
                    .Include(a=>a.Staff)
                    .ProjectTo<BookingDto>(_mapper.ConfigurationProvider) 
                    .ToListAsync(cancellationToken);

                return Result<Pagination<BookingDto>>.Success(new Pagination<BookingDto>(querySpec.PageSize, totalElement, data));
            }
        }

    }
}