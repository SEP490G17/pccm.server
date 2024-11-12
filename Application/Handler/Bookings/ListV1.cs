using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public class Query : IRequest<Result<Pagination<BookingDtoV1>>>
        {
            public BookingSpecParam BookingSpecParam { get; set; }
        }

        public class Handler(IMapper _mapper, IUnitOfWork _unitOfWork) : IRequestHandler<Query, Result<Pagination<BookingDtoV1>>>
        {
            public async Task<Result<Pagination<BookingDtoV1>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var querySpec = request.BookingSpecParam;

                var spec = new BookingV1Specification(querySpec);
                var specCount = new BookingV1CountSpecification(querySpec);

                var totalElement = await _unitOfWork.Repository<Booking>().CountAsync(specCount, cancellationToken);

                var data = await _unitOfWork.Repository<Booking>()
                    .QueryList(spec)
                    .ProjectTo<BookingDtoV1>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);
                return Result<Pagination<BookingDtoV1>>.Success(new Pagination<BookingDtoV1>(querySpec.PageSize, totalElement, data));
            }
        }
    }
}