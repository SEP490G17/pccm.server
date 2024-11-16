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
    public class AcceptedBookingHandlerTests
    {
        private readonly IMediator Mediator;

        public AcceptedBookingHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(36, ExpectedResult = true)]
        public async Task<bool> Handle_ShouldAcceptBooking_WhenValidData(
            int id)
        {
            try
            {
                var result = await Mediator.Send(new AcceptedBooking.Command() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

      [TestCase(111, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldAcceptBookingFail_WhenNotExistBooking(
            int id)
        {
            try
            {
                var result = await Mediator.Send(new AcceptedBooking.Command() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

         [TestCase(26, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldAcceptBookingFail_WhenBookingOverlaps(
            int id)
        {
            try
            {
                var result = await Mediator.Send(new AcceptedBooking.Command() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
       
    }
}
