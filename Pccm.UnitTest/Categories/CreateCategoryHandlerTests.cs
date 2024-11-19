﻿using Application.Core;
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
using Application.Handler.Categories;

namespace Pccm.UnitTest.Categories
{
    [TestFixture]
    public class CreateCategoryHandlerTests
    {
        private readonly IMediator Mediator;

        public CreateCategoryHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase("Sports", ExpectedResult = true)]
        public async Task<bool> Handle_CreateCategory_WhenValid(
          string CategoryName)
        {
            try
            {
                var category = new Category
                {
                    CategoryName = CategoryName
                };

                var result = await Mediator.Send(new Create.Command() { Category = category }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        [TestCase(null, ExpectedResult = false)]
        public async Task<bool> Handle_ShouldCreateCategoryFail_WhenNameIsNull(
          string? CategoryName)
        {
            try
            {
                var category = new Category
                {
                    CategoryName = CategoryName
                };

                var result = await Mediator.Send(new Create.Command() { Category = category }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
