using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.DTOs;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.Extensions.Options;

namespace API.Services
{

    public class VnPayService : IVnPayService
    {
        private readonly VnPaySettings _settings;

        public VnPayService(IOptions<VnPaySettings> settings)
        {
            _settings = settings.Value;
        }

        public string GeneratePaymentUrl(int bookingId, decimal amount)
        {
            var vnpayParams = new SortedDictionary<string, string>
    {
        { "vnp_Version", "2.1.0" },
        { "vnp_Command", "pay" },
        { "vnp_TmnCode", _settings.TmnCode },
        { "vnp_Amount", ((int)(amount * 100)).ToString() },  // Đảm bảo amount đúng
        { "vnp_CurrCode", "VND" },
        { "vnp_TxnRef", bookingId.ToString() },
        { "vnp_OrderInfo", Uri.EscapeDataString($"Booking {bookingId}") },
        { "vnp_OrderType", "billpayment" },
        { "vnp_ReturnUrl", _settings.ReturnUrl },
        { "vnp_Locale", "vn" },
        { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") }
    };

            var hashData = string.Join("&", vnpayParams.Select(p => $"{p.Key}={p.Value}"));

            // In giá trị hashData để kiểm tra
            Console.WriteLine("HashData: " + hashData);

            vnpayParams.Add("vnp_SecureHash", ComputeHash(hashData, _settings.HashSecret));

            // In URL để kiểm tra
            var paymentUrl = $"{_settings.Url}?{string.Join("&", vnpayParams.Select(p => $"{p.Key}={p.Value}"))}";
            Console.WriteLine("Payment URL: " + paymentUrl);

            return paymentUrl;
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

            var hash = ComputeHash(string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}")), _settings.HashSecret);
            return hash.Equals(callback.vnp_SecureHash);
        }

        private string ComputeHash(string data, string secret)
        {
            var byteArray = Encoding.UTF8.GetBytes(data + secret);
            using (var sha256 = SHA256.Create())
            {
                return BitConverter.ToString(sha256.ComputeHash(byteArray)).Replace("-", "").ToLower();
            }
        }
    }


}