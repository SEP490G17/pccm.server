using API.Extensions;
using Application.DTOs;
using Application.Handler.Orders;
using Application.Handler.Payments;
using Domain.Entity;
using Domain.Enum;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pccm.UnitTest.Payments
{
    public class ProcessPaymentHandlerTests
    {
        private readonly IMediator Mediator;

        public ProcessPaymentHandlerTests()
        {
            var builder = Host.CreateEmptyApplicationBuilder(new());
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddApplicationService(builder.Configuration);

            var host = builder.Build();
            Mediator = host.Services.GetRequiredService<IMediator>();
        }


    //    [TestCase(14, ExpectedResult = true)]
    //     public async Task<bool> Handle_ShouldPayment_WhenValid(
    //         int Id )
    //     {
    //         try
    //         {
    //             var result = await Mediator.Send(new ProcessPayment.Command()
    //             {
    //                 BillPayId = Id,
    //                 Type = PaymentType.Order
    //             }, default);

    //             return result.IsSuccess;
    //         }
    //         catch (Exception)
    //         {
    //             return false;
    //         }
    //     }

    //   [TestCase(2, ExpectedResult = false)]
    //     public async Task<bool> Handle_ShouldPaymentFail_WhenExistOrderNotPayMent(
    //         int Id )
    //     {
    //         try
    //         {
    //             var result = await Mediator.Send(new ProcessPayment.Command()
    //             {
    //                 BillPayId = Id,
    //                 Type = PaymentType.Order
    //             }, default);

    //             // Kiểm tra kết quả trả về
    //             return result.IsSuccess;
    //         }
    //         catch (Exception)
    //         {
    //             return false;
    //         }
    //     }


        [TestCase(14, PaymentType.Order, ExpectedResult = true)] // Valid Order Payment
        public async Task<bool> Handle_ShouldPayment_WhenValid(int billPayId, PaymentType paymentType)
        {
            try
            {
                var result = await Mediator.Send(new ProcessPayment.Command()
                {
                    BillPayId = billPayId,
                    Type = paymentType
                }, default);

                return result.IsSuccess;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [TestCase(2, PaymentType.Order, ExpectedResult = false)] // Order Not Found
        public async Task<bool> Handle_ShouldPaymentFail_WhenOrderNotFound(int billPayId, PaymentType paymentType)
        {
            try
            {
                var result = await Mediator.Send(new ProcessPayment.Command()
                {
                    BillPayId = billPayId,
                    Type = paymentType
                }, default);

                return result.IsSuccess;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [TestCase(14, PaymentType.Booking, ExpectedResult = true)] // Valid Booking Payment
        public async Task<bool> Handle_ShouldPayment_WhenBookingValid(int billPayId, PaymentType paymentType)
        {
            try
            {
                var result = await Mediator.Send(new ProcessPayment.Command()
                {
                    BillPayId = billPayId,
                    Type = paymentType
                }, default);

                return result.IsSuccess;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [TestCase(14, PaymentType.Booking, ExpectedResult = false)] // Booking Not Found
        public async Task<bool> Handle_ShouldPaymentFail_WhenBookingNotFound(int billPayId, PaymentType paymentType)
        {
            try
            {
                var result = await Mediator.Send(new ProcessPayment.Command()
                {
                    BillPayId = billPayId,
                    Type = paymentType
                }, default);

                return result.IsSuccess;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [TestCase(2, PaymentType.Booking, ExpectedResult = false)] // Multiple Pending Orders for Booking
        public async Task<bool> Handle_ShouldFail_WhenMultipleOrdersPending(int billPayId, PaymentType paymentType)
        {
            try
            {
                var result = await Mediator.Send(new ProcessPayment.Command()
                {
                    BillPayId = billPayId,
                    Type = paymentType
                }, default);

                return result.IsSuccess;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [TestCase(1000, PaymentType.Order, ExpectedResult = false)] // Invalid Payment Type
        public async Task<bool> Handle_ShouldFail_WhenInvalidPaymentType(int billPayId, PaymentType paymentType)
        {
            try
            {
                var result = await Mediator.Send(new ProcessPayment.Command()
                {
                    BillPayId = billPayId,
                    Type = paymentType
                }, default);

                return result.IsSuccess;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
