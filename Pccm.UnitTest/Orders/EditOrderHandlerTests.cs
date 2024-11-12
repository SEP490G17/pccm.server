using API.Extensions;
using Application.Core;
using Application.DTOs;
using Application.Handler.Orders;
using AutoMapper;
using Domain.Entity;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using NUnit.Framework;
using Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pccm.UnitTest.Orders
{
    public class EditOrderHandlerTests
    {
        private readonly IMediator Mediator;

        public EditOrderHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(7,4, 1, "2024-11-11T10:00:00", "2024-11-11T12:00:00", 1000.00, "Pending", ExpectedResult = true)]
        public async Task<bool> Handle_EditOrder_WhenValid(
            int id,
            int BookingId,
            int StaffId,
            string StartTime,
            string EndTime,
            decimal TotalAmount,
            string Status)
        {
            try
            {
                var orderInputDto = new OrderInputDto()
                {
                    Id = id,
                    BookingId = BookingId,
                    StaffId = StaffId,
                    StartTime = DateTime.Parse(StartTime),
                    EndTime = DateTime.Parse(EndTime),
                    TotalAmount = TotalAmount,
                    Status = Status
                };

                var result = await Mediator.Send(new Edit.Command() { order = orderInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
