using API.DTOs;
using API.Helper;
using Application.DTOs;
using Application.Interfaces;
using Domain.Enum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Infrastructure.Payment
{
    public class VnPayService : IVnPayService
    {
        private readonly VnPaySettings _settings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _env;
        private readonly IUserAccessor _userAccessor;


        public VnPayService(IOptions<VnPaySettings> settings, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env, IUserAccessor userAccessor)
        {
            _settings = settings.Value;
            _httpContextAccessor = httpContextAccessor;
            _env = env;
            _userAccessor = userAccessor;
        }

        public string GeneratePaymentUrl(int billPaymentId, decimal amount, PaymentType type)
        {
            var tick = DateTime.Now.ToString();
            var ipAddress = Utils.GetIpAddress(_httpContextAccessor.HttpContext);
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Bangkok");

            var utcNow = DateTime.UtcNow;
            var createDate = TimeZoneInfo.ConvertTimeFromUtc(utcNow, vietnamTimeZone);

            var vnPay = new VnPayLibrary();
            vnPay.AddRequestData("vnp_Version", "2.1.1");
            vnPay.AddRequestData("vnp_Command", "pay");
            vnPay.AddRequestData("vnp_TmnCode", _settings.TmnCode);
            vnPay.AddRequestData("vnp_Amount", ((int)(amount * 100)).ToString());
            vnPay.AddRequestData("vnp_CreateDate", createDate.ToString("yyyyMMddHHmmss"));
            vnPay.AddRequestData("vnp_CurrCode", "VND");
            vnPay.AddRequestData("vnp_IpAddr", string.IsNullOrEmpty(ipAddress) ? "127.0.0.1" : ipAddress);
            vnPay.AddRequestData("vnp_Locale", "vn");
            vnPay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang: {billPaymentId}");
            vnPay.AddRequestData("vnp_OrderType", $"{type.ToString()}");
            vnPay.AddRequestData("vnp_ReturnUrl", _env.IsProduction()? _settings.ReturnUrl:"http://localhost:5000/api/payment/vnpay-callback");
            vnPay.AddRequestData("vnp_ExpireDate", createDate.AddMinutes(30).ToString("yyyyMMddHHmmss"));
            vnPay.AddRequestData("vnp_TxnRef", $"{billPaymentId}_{(int)type}_{_userAccessor.GetUserName()}_{tick}");
            return vnPay.CreateRequestUrl(_settings.Url, _settings.HashSecret);
        }

        public bool VerifyVnPaySignature(VnPayCallbackDto callback)
        {
            var library = new VnPayLibrary();
            library.AddResponseData("vnp_TmnCode", callback.vnp_TmnCode);
            library.AddResponseData("vnp_Amount", callback.vnp_Amount);
            library.AddResponseData("vnp_BankCode", callback.vnp_BankCode);
            library.AddResponseData("vnp_BankTranNo", callback.vnp_BankTranNo);
            library.AddResponseData("vnp_CardType", callback.vnp_CardType);
            library.AddResponseData("vnp_PayDate", callback.vnp_PayDate);
            library.AddResponseData("vnp_OrderInfo", callback.vnp_OrderInfo);
            library.AddResponseData("vnp_TransactionNo", callback.vnp_TransactionNo);
            library.AddResponseData("vnp_ResponseCode", callback.vnp_ResponseCode);
            library.AddResponseData("vnp_TxnRef", callback.vnp_TxnRef);
            library.AddResponseData("vnp_SecureHash", callback.vnp_SecureHash);

            
            return library.ValidateSignature(callback.vnp_SecureHash, _settings.HashSecret);
        }


    }
}
