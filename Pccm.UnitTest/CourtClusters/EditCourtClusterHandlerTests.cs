using API.Extensions;
using Application.Core;
using Application.DTOs;
using Application.Handler.Courts;
using AutoMapper;
using Domain.Entity;
using Domain.Enum;
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

namespace Pccm.UnitTest.CourtClusters
{
    public class EditCourtClusterHandlerTests
    {
        private readonly IMediator Mediator;

        public EditCourtClusterHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


       [TestCase(4, "Premium Court 2", 1, CourtStatus.Available, ExpectedResult = true)]
        public async Task<bool> Handle_EditCourt_WhenValid(
            int id,
                string CourtName,
                int? CourtClusterId,
                CourtStatus Status)
        {
            try
            {
                var courtInputDto = new Court
                {
                    Id = id,
                    CourtName = CourtName, 
                    CourtClusterId = CourtClusterId, 
                    Status = Status
                };

                var result = await Mediator.Send(new Edit.Command() { court = courtInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

          [TestCase(14, "Premium Court 2", 1, CourtStatus.Available, ExpectedResult = false)]
        public async Task<bool> Handle_EditCourt_WhenNotExistCourt(
            int id,
                string CourtName,
                int? CourtClusterId,
                CourtStatus Status)
        {
            try
            {
                var courtInputDto = new Court
                {
                    Id = id,
                    CourtName = CourtName, 
                    CourtClusterId = CourtClusterId, 
                    Status = Status
                };

                var result = await Mediator.Send(new Edit.Command() { court = courtInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
