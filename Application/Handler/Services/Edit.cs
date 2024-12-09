using System.Globalization;
using Application.Core;
using Application.DTOs;
using Application.Handler.StaffPositions;
using Application.Interfaces;
using AutoMapper;
using Domain.Entity;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Handler.Services
{
    public class Edit
    {
        public class Command : IRequest<Result<ServiceDto>>
        {
            public ServiceInputDto Service { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Service).SetValidator(new ServiceValidator());
            }
        }
        public class Handler(DataContext _context, IMapper _mapper, IUserAccessor _userAccessor, IMediator _mediator) : IRequestHandler<Command, Result<ServiceDto>>
        {


            public async Task<Result<ServiceDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var serviceExist = await _context.Services.Include(s => s.CourtCluster)
                .FirstOrDefaultAsync(s => s.Id == request.Service.Id && s.DeletedAt == null, cancellationToken);
                if (serviceExist == null)
                    return Result<ServiceDto>.Failure("Dịch vụ không tồn tại hoặc đã bị xoá");
                List<int> clusterIds = await _mediator.Send(new GetCurrentStaffCluster.Query(), cancellationToken);
                if (clusterIds != null && !clusterIds.Contains((int)serviceExist.CourtClusterId))
                {
                    return Result<ServiceDto>.Failure("Bạn không có quyền thực hiện quyền này");
                }
                _mapper.Map(request.Service, serviceExist);
                serviceExist.UpdatedAt = DateTime.Now;
                serviceExist.UpdatedBy = "Anonymous";
                string description = "đã thay đổi ";
                var cultureInfo = (CultureInfo)CultureInfo.InvariantCulture.Clone();
                cultureInfo.NumberFormat.CurrencySymbol = "₫";
                cultureInfo.NumberFormat.CurrencyDecimalDigits = 0;
                cultureInfo.NumberFormat.NumberGroupSeparator = ".";
                cultureInfo.NumberFormat.CurrencyGroupSeparator = ".";
                if (serviceExist.ServiceName != request.Service.ServiceName)
                {
                    description += $"tên dịch vụ {serviceExist.ServiceName} thành {request.Service.ServiceName} ";
                }
                else if (serviceExist.Description != request.Service.Description)
                {

                    description += $"mô tả {serviceExist.Description} thành {request.Service.Description} ";
                }
                else if (serviceExist.Price != request.Service.Price)
                {
                    description += $"giá bán {string.Format(cultureInfo, "{0:C}", serviceExist.Price)} thành {string.Format(cultureInfo, "{0:C}", request.Service.Price)} ";
                }
                _context.Services.Update(serviceExist);
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                var serviceLog = _mapper.Map<ServiceLog>(serviceExist);
                serviceLog.Id = 0;
                serviceLog.CreatedAt = DateTime.Now;
                serviceLog.CreatedBy = _userAccessor.GetUserName();
                serviceLog.Price = serviceExist.Price;
                serviceLog.LogType = Domain.Enum.LogType.Update;
                serviceLog.CourtCluster = await _context.CourtClusters.FirstOrDefaultAsync(c => c.Id == serviceExist.CourtClusterId);
                serviceLog.Description = description;
                await _context.ServiceLogs.AddAsync(serviceLog, cancellationToken);
                var _result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result || !_result) return Result<ServiceDto>.Failure("Fail to edit service");
                var response = _mapper.Map<ServiceDto>(serviceExist);
                return Result<ServiceDto>.Success(response);
            }
        }
    }
}
