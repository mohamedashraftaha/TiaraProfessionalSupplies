using TiaraPro.Server.Models;
using TiaraPro.Server.PersistenceLayer.UnitOfWork;

namespace TiaraPro.Server.Services.Payments;

public class PaymentsService : IPaymentsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PaymentsService> _logger;
    public PaymentsService(IUnitOfWork unitOfWork, ILogger<PaymentsService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<bool> CreatePaymentAsync(Payment payment)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var result = await _unitOfWork.Payments.CreatePaymentAsync(payment);
            await _unitOfWork.CompleteAsync();
            return result;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Error creating payment.");
            throw;
        }
    }

    public async Task<List<Payment?>?> GetPayments()
    {
        try
        {
            return await _unitOfWork.Payments.GetPayments();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payment.");
            return new List<Payment?>();

        }
    }
    public async Task<Payment?> GetPaymentByIdAsync(int paymentId)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var payment = await _unitOfWork.Payments.GetPaymentByIdAsync(paymentId);
            if (payment == null)
            {
                _logger.LogWarning("Payment not found with ID: {PaymentId}", paymentId);
                return null;
            }
            await _unitOfWork.CompleteAsync();
            return payment;
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error retrieving payment by ID.");
            // Log the exception
            Console.WriteLine(ex.Message);
            return null;
        }
    }
    public async Task<Payment?> GetPaymentsByTransactionIdAsync(int transactionId)
    {
        return await _unitOfWork.Payments.GetPaymentsByTransactionIdAsync(transactionId);
    }
    public async Task<bool> UpdatePaymentStatusAsync(int paymentId, string status)
    {
        try
        {

            await _unitOfWork.BeginTransactionAsync();
            var payment = await _unitOfWork.Payments.GetPaymentsByTransactionIdAsync(paymentId);
            if (payment == null)
            {
                _logger.LogWarning("Payment not found with ID: {PaymentId}", paymentId);
                return false;
            }
            payment.PaymentStatus = status;
            var result = await _unitOfWork.Payments.UpdatePaymentStatusAsync(paymentId, status);
            if (result)
            {
                await _unitOfWork.CompleteAsync();
            }

            return result;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Error updating payment status.");
            throw;

        }
    }
    public async Task<bool> DeletePaymentAsync(int paymentId)
    {
        return await _unitOfWork.Payments.DeletePaymentAsync(paymentId);
    }
    public async Task<bool> UpdatePaymentAsync(Payment payment)
    {
        return await _unitOfWork.Payments.UpdatePaymentAsync(payment);
    }
}
