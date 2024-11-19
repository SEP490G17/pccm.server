using API.Extensions;
using Application.Core;
using Application.DTOs;
using Application.Handler.Services;
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

namespace Pccm.UnitTest.Services
{
    public class EditBannerHandlerTests
    {
        private readonly IMediator Mediator;

        public EditBannerHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


       [TestCase(14, new int[] { 2 }, "Premium Service", "High-quality tennis court rental", 150.00, ExpectedResult = true)]
        public async Task<bool> Handle_EditService_WhenValid(
            int Id,
            int[] CourtClusterId,
            string ServiceName,
            string Description,
            decimal Price)
        {
            try
            {
                var serviceInputDto = new ServiceInputDto()
                {
                    Id = Id,
                    CourtClusterId = CourtClusterId,
                    ServiceName = ServiceName,
                    Description = Description,
                    Price = Price
                };

                var result = await Mediator.Send(new Edit.Command() { Service = serviceInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase(140, new int[] { 2 }, "Premium Service", "High-quality tennis court rental", 150.00, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldEditServiceFail_WhenNotExistID(
            int Id,
            int[] CourtClusterId,
            string ServiceName,
            string Description,
            decimal Price)
        {
            try
            {
                var serviceInputDto = new ServiceInputDto()
                {
                    Id = Id,
                    CourtClusterId = CourtClusterId,
                    ServiceName = ServiceName,
                    Description = Description,
                    Price = Price
                };

                var result = await Mediator.Send(new Edit.Command() { Service = serviceInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
