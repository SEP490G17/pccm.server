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
    public class EditUserHandlerTests
    {
        private readonly IMediator Mediator;

        public EditUserHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase("john_doe", "john.doe@example.com", "1234567890", true, "1990-01-01", "123 Main St", ExpectedResult = true)]
        public async Task<bool> Handle_EditUser_WhenValid(
            string username,
            string email,
            string phoneNumber,
            bool? gender,
            string birthDate,
            string address)
        {
            try
            {
                var profileInputDto = new ProfileInputDto()
                {
                    Username = username,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    Gender = gender,
                    BirthDate = string.IsNullOrWhiteSpace(birthDate) ? null : DateTime.Parse(birthDate),
                    Address = address
                };

                var appUser = new AppUser()
                {
                  Id = "06b243a8-8158-4a9c-845e-63054506a1b8"
                };

                var result = await Mediator.Send(new EditProfile.Command() { ProfileInputDto = profileInputDto, User = appUser }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

          [TestCase("john_doe", "john.doe@example.com", "1234567890", true, "1990-01-01", "123 Main St", ExpectedResult = false)]
        public async Task<bool> Handle_EditUser_WhenNotExistUserID(
            string username,
            string email,
            string phoneNumber,
            bool? gender,
            string birthDate,
            string address)
        {
            try
            {
                var profileInputDto = new ProfileInputDto()
                {
                    Username = username,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    Gender = gender,
                    BirthDate = string.IsNullOrWhiteSpace(birthDate) ? null : DateTime.Parse(birthDate),
                    Address = address
                };

                var appUser = new AppUser()
                {
                  Id = "06b243a8-8158-4a9c-845e-63054506a1b89"
                };

                var result = await Mediator.Send(new EditProfile.Command() { ProfileInputDto = profileInputDto, User = appUser }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


    }
}
