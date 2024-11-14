using Application.DTOs;
using Domain.Enum;

namespace Application.Interfaces
{
    public interface IVnPayService
    {
        string GeneratePaymentUrl(int bookingId, decimal amount, PaymentType type);
        bool VerifyVnPaySignature(VnPayCallbackDto callback);
    }
}