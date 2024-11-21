using Application.Core;
using Application.Handler.Bookings;
using AutoMapper;
using Domain.Entity;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Persistence;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using API.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Application.DTOs;

namespace Pccm.UnitTest.Bookings
{
    [TestFixture]
    public class CancelBookingHandlerTests
    {
        private readonly IMediator Mediator;

        public CancelBookingHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(4, ExpectedResult = true)]
        public async Task<bool> Handle_ShouldCancelBooking_WhenValidData(
            int id)
        {
            try
            {
                var result = await Mediator.Send(new CancelBooking.Command() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

      [TestCase(111, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldCancelBookingFail_WhenNotExistBooking(
            int id)
        {
            try
            {
                var result = await Mediator.Send(new CancelBooking.Command() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

         [TestCase(3, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldCancelBookingFail_WhenBookingIsSuccess(
            int id)
        {
            try
            {
                var result = await Mediator.Send(new CancelBooking.Command() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
       
    }
}
