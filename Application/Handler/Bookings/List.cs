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
using Persistence;

namespace Application.Handler.Bookings
{
    public class List
    {
        public class Query : IRequest<Result<Pagination<BookingDTO>>>
        {
            public BaseSpecWithFilterParam BaseSpecWithFilterParam { get; set; }
        }

        public class Handler(IMapper _mapper, IUnitOfWork _unitOfWork) : IRequestHandler<Query, Result<Pagination<BookingDTO>>>
        {
            public async Task<Result<Pagination<BookingDTO>>> Handle(Query request, CancellationToken cancellationToken)
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
                    .ProjectTo<BookingDTO>(_mapper.ConfigurationProvider) 
                    .ToListAsync(cancellationToken);

                return Result<Pagination<BookingDTO>>.Success(new Pagination<BookingDTO>(querySpec.PageSize, totalElement, data));
            }
        }

    }
}