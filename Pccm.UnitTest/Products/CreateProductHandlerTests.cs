using Application.Core;
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
using Application.Handler.Products;
using Domain.Enum;

namespace Pccm.UnitTest.Products
{
    [TestFixture]
    public class CreateProductHandlerTests
    {
        private readonly IMediator Mediator;

        public CreateProductHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(1, 1, "Premium Product", "High-quality tennis balls", 100, 50.5, 30.0, "url-to-thumbnail", ExpectedResult = true)]
        public async Task<bool> Handle_CreateProduct_WhenValid(
        int CategoryId,
        int CourtClusterId,
        string ProductName,
        string Description,
        int Quantity,
        decimal PriceSell,
        decimal ImportFee,
        string ThumbnailUrl)
        {
            try
            {
                var productInputDto = new ProductInputDto
                {
                    CategoryId = CategoryId,
                    CourtClusterId = CourtClusterId,
                    ProductName = ProductName,
                    Description = Description,
                    Quantity = Quantity,
                    Price = PriceSell,
                    ImportFee = ImportFee,
                    ThumbnailUrl = ThumbnailUrl
                };

                var result = await Mediator.Send(new Create.Command { product = productInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        [TestCase(100, 1, "", "High-quality tennis balls", 100, 50.5, 30.0, "url-to-thumbnail", ExpectedResult = false)]
        public async Task<bool> Handle_CreateProduct_WhenNotExistCategory(
        int CategoryId,
        int CourtClusterId,
        string ProductName,
        string Description,
        int Quantity,
        decimal PriceSell,
        decimal ImportFee,
        string ThumbnailUrl)
        {
            try
            {
                var productInputDto = new ProductInputDto
                {
                    CategoryId = CategoryId,
                    CourtClusterId = CourtClusterId,
                    ProductName = ProductName,
                    Description = Description,
                    Quantity = Quantity,
                    Price = PriceSell,
                    ImportFee = ImportFee,
                    ThumbnailUrl = ThumbnailUrl
                };

                var result = await Mediator.Send(new Create.Command { product = productInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                // Return false if any exception occurs
                return false;
            }
        }

        [TestCase(1, 100, "", "High-quality tennis balls", 100, 50.5, 30.0, "url-to-thumbnail", ExpectedResult = false)]
        public async Task<bool> Handle_CreateProduct_WhenNotExistCourtClusterID(
        int CategoryId,
        int CourtClusterId,
        string ProductName,
        string Description,
        int Quantity,
        decimal PriceSell,
        decimal ImportFee,
        string ThumbnailUrl)
        {
            try
            {
                var productInputDto = new ProductInputDto
                {
                    CategoryId = CategoryId,
                    CourtClusterId = CourtClusterId,
                    ProductName = ProductName,
                    Description = Description,
                    Quantity = Quantity,
                    Price = PriceSell,
                    ImportFee = ImportFee,
                    ThumbnailUrl = ThumbnailUrl
                };

                var result = await Mediator.Send(new Create.Command { product = productInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                // Return false if any exception occurs
                return false;
            }
        }
    }
}
