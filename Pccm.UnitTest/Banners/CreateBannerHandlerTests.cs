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
using Application.Handler.Banners;
using Domain.Enum;

namespace Pccm.UnitTest.Banners
{
    [TestFixture]
    public class CreateBannerHandlerTests
    {
        private readonly IMediator Mediator;

        public CreateBannerHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase("Premium Service 2", "High-qualitl", 150, "https://example.com/image.jpg", "https://example.com", "2024-11-01", "2024-12-01", BannerStatus.Display, BannerType.Banner, BannerInPage.HomePage, ExpectedResult = true)]
        public async Task<bool> Handle_CreateBanner_WhenValid(
             string Title,
             string Description,
             decimal Price,
             string ImageUrl,
             string LinkUrl,
             string StartDate,
             string EndDate,
             BannerStatus Status,
             BannerType BannerType,
             BannerInPage BannerInPage)
        {
            try
            {
                var bannerInputDto = new BannerInputDto()
                {
                    Title = Title,
                    Description = Description,
                    ImageUrl = ImageUrl,
                    LinkUrl = LinkUrl,
                    StartDate = DateTime.Parse(StartDate),
                    EndDate = DateTime.Parse(EndDate),
                    Status = Status,
                    BannerType = BannerType,
                    BannerInPage = BannerInPage
                };

                var result = await Mediator.Send(new Create.Command() { Banner = bannerInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        [TestCase(null, "High-qualitl", 150, "https://example.com/image.jpg", "https://example.com", "2024-11-01", "2024-12-01", BannerStatus.Display, BannerType.Banner, BannerInPage.HomePage, ExpectedResult = false)]
        public async Task<bool> Handle_CreateBanner_WhenNotInputTitle(
             string? Title,
             string Description,
             decimal Price,
             string ImageUrl,
             string LinkUrl,
             string StartDate,
             string EndDate,
             BannerStatus Status,
             BannerType BannerType,
             BannerInPage BannerInPage)
        {
            try
            {
                var bannerInputDto = new BannerInputDto()
                {
                    Title = Title,
                    Description = Description,
                    ImageUrl = ImageUrl,
                    LinkUrl = LinkUrl,
                    StartDate = DateTime.Parse(StartDate),
                    EndDate = DateTime.Parse(EndDate),
                    Status = Status,
                    BannerType = BannerType,
                    BannerInPage = BannerInPage
                };

                var result = await Mediator.Send(new Create.Command() { Banner = bannerInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
