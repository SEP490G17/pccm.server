using API.Extensions;
using Application.Core;
using Application.DTOs;
using Application.Handler.Users;
using AutoMapper;
using Domain;
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

namespace Pccm.UnitTest.Users
{
    public class ActiveUserHandlerTests
    {
        private readonly IMediator Mediator;

        public ActiveUserHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase("16d78ef3-0524-4fbb-860f-de5e91a67a1d", true, ExpectedResult = true)]
        public async Task<bool> Handle_ActiveUser_WhenValid(
         string id,
         bool isActive)
              {
            try
            {
                var activeDto = new ActiveDto()
                {
                    Id = id,
                    IsActive = isActive
                };

                var result = await Mediator.Send(new ActiveUser.Command() { user = activeDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

         [TestCase("06b243a8-8158-4a9c-845e-63054506a1b89", true, ExpectedResult = false)]
        public async Task<bool> Handle_ActiveUser_WhenNotExistUserID(
         string id,
         bool isActive)
        {
            try
            {
                var activeDto = new ActiveDto()
                {
                    Id = id,
                    IsActive = isActive
                };

                var result = await Mediator.Send(new ActiveUser.Command() { user = activeDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
