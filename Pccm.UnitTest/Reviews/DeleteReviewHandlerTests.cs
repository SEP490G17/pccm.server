using MediatR;
using NUnit.Framework;
using Persistence;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Domain.Entity;
using Moq;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using API.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Application.Handler.Reviews;

namespace Pccm.UnitTest.Reviews
{
    public class DeleteReviewHandlerTests
    {
          private readonly IMediator Mediator;

        public DeleteReviewHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


        [TestCase(2, ExpectedResult = true)]
        public async Task<bool> Handle_DeleteReview_WhenValidId(
            int id)
        {
            try
            {
                var result = await Mediator.Send(new Delete.Command() { Id = id }, default);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
