using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Helper;
using Application.DTOs;
using Application.Interfaces;
using Domain.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Payment
{
    public class VnPayService : IVnPayService
    {
        private readonly VnPaySettings _settings;

        public VnPayService(IOptions<VnPaySettings> settings)
        {
            _settings = settings.Value;
        }

        public string GeneratePaymentUrl(int billPaymentId, decimal amount, PaymentType type)
        {
            var tick = DateTime.Now.ToString();
     
            var createDate = DateTime.Now;
            var vnPay = new VnPayLibrary();
            vnPay.AddRequestData("vnp_Version", "2.1.1");
            vnPay.AddRequestData("vnp_Command", "pay");
            vnPay.AddRequestData("vnp_TmnCode", _settings.TmnCode);
            vnPay.AddRequestData("vnp_Amount", ((int)(amount * 100)).ToString());
            vnPay.AddRequestData("vnp_CreateDate",createDate.ToString("yyyyMMddHHmmss"));
            vnPay.AddRequestData("vnp_ExpireDate",createDate.AddMinutes(30).ToString("yyyyMMddHHmmss"));
            vnPay.AddRequestData("vnp_CurrCode", "VND");
            vnPay.AddRequestData("vnp_TxnRef", $"{billPaymentId}_{type}_{tick}");
         
            vnPay.AddRequestData("vnp_Locale", "vn");
            vnPay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang: {billPaymentId}");
            vnPay.AddRequestData("vnp_OrderType", "bookingBill");
            vnPay.AddRequestData("vnp_ReturnUrl", _settings.ReturnUrl);
            vnPay.AddRequestData("vnp_IpAddr", "127.0.0.1");

            return vnPay.CreateRequestUrl(_settings.Url,_settings.HashSecret);
        }

        public bool VerifyVnPaySignature(VnPayCallbackDto callback)
        {
            var parameters = new SortedDictionary<string, string>
            {
                { "vnp_Amount", callback.vnp_Amount },
                { "vnp_BankCode", callback.vnp_BankCode },
                { "vnp_OrderInfo", callback.vnp_OrderInfo },
                { "vnp_ResponseCode", callback.vnp_ResponseCode },
                { "vnp_TmnCode", callback.vnp_TmnCode },
                { "vnp_TransactionNo", callback.vnp_TransactionNo },
                { "vnp_TxnRef", callback.vnp_TxnRef }
            };

            // var hash = ComputeHash(string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}")), _settings.HashSecret);
            // return hash.Equals(callback.vnp_SecureHash);
            return true;
        }


    }
}