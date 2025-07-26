using TiaraPro.Server.Models;

namespace TiaraPro.Server.Services.Payments;

public interface IPaymentsService
{
    Task<bool> CreatePaymentAsync(Payment payment);
    Task<Payment?> GetPaymentByIdAsync(int paymentId);
    Task<Payment?> GetPaymentsByTransactionIdAsync(int transactionId);

    Task<List<Payment?>?> GetPayments();
    Task<bool> UpdatePaymentStatusAsync(int paymentId, string status);
    Task<bool> DeletePaymentAsync(int paymentId);
    Task<bool> UpdatePaymentAsync(Payment payment);
}
