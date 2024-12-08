using API.Extensions;
using Application.Core;
using Application.DTOs;
using Application.Handler.Notifications;
using Application.Interfaces;
using Application.SpecParams;
using Application.SpecParams.NotificationSpecification;
using AutoMapper;
using Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
namespace Pccm.UnitTest.Notifications
{
    [TestFixture]
    public class UpdateReadAtNotificationHandlerTests
    {
        public readonly IMediator Mediator;

        public UpdateReadAtNotificationHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(0, 5,3, ExpectedResult = true)]
        public async Task<bool?> Handle_ShouldUpdateNotification_WhenValidId(int skip, int pageSize, int id)
        {
            if (this.Mediator is null) return null;
            var response = await this.Mediator.Send(new Application.Handler.Notifications.UpdateReadAtNotification.Command()
            {
                NotiId = id
            });

            return response.IsSuccess;
        }

    }
}