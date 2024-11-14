using API.Extensions;
using Application.Core;
using Application.DTOs;
using Application.Handler.Reviews;
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

namespace Pccm.UnitTest.Reviews
{
    public class EditServiceHandlerTests
    {
        private readonly IMediator Mediator;

        public EditServiceHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(2,"0327aee1-dc23-46f1-8a5b-343d669afd85", 4, 5, "Great service!", "2024-11-11T10:00:00", ExpectedResult = true)]
        public async Task<bool> Handle_EditReview_WhenValid(
            int id, 
            string UserId,
            int CourtClusterId,
            int Rating,
            string Comment,
            string CreatedAt)
        {
            try
            {
                var reviewInputDto = new ReviewInputDto()
                {
                    Id = id,
                    UserId = UserId,
                    CourtClusterId = CourtClusterId,
                    Rating = Rating,
                    Comment = Comment,
                    CreatedAt = DateTime.Parse(CreatedAt)
                };

                var result = await Mediator.Send(new Edit.Command() { review = reviewInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
