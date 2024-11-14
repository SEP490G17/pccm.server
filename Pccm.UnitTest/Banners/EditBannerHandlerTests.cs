using API.Extensions;
using Application.Core;
using Application.DTOs;
using Application.Handler.Banners;
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

namespace Pccm.UnitTest.Banners
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


        [TestCase(4, "Premium Service 2", "High-qualitl", 150, "https://example.com/image.jpg", "https://example.com", "2024-11-01", "2024-12-01", BannerStatus.Display, BannerType.Banner, BannerInPage.HomePage, ExpectedResult = true)]
        public async Task<bool> Handle_EditBanner_WhenValid(
             int id,
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
                    Id = id,
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

                var result = await Mediator.Send(new Edit.Command() { Banner = bannerInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase(140, "Premium Service 2", "High-qualitl", 150, "https://example.com/image.jpg", "https://example.com", "2024-11-01", "2024-12-01", BannerStatus.Display, BannerType.Banner, BannerInPage.HomePage, ExpectedResult = false)]
        public async Task<bool> Handle_EditBanner_WhenNotExistBanner(
                 int id,
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
                    Id = id,
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

                var result = await Mediator.Send(new Edit.Command() { Banner = bannerInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [TestCase(14, null, "High-qualitl", 150, "https://example.com/image.jpg", "https://example.com", "2024-11-01", "2024-12-01", BannerStatus.Display, BannerType.Banner, BannerInPage.HomePage, ExpectedResult = false)]
        public async Task<bool> Handle_EditBanner_WhenTitleIsNull(
                 int id,
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
                    Id = id,
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

                var result = await Mediator.Send(new Edit.Command() { Banner = bannerInputDto }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
