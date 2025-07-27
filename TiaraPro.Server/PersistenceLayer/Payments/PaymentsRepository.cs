using Microsoft.EntityFrameworkCore;
using TiaraPro.Server.Models;

namespace TiaraPro.Server.PersistenceLayer.Payments;

public class PaymentsRepository : IPaymentsRepository
{
    private readonly TiaraDbContext _context;
    private readonly ILogger<PaymentsRepository> _logger;
    public PaymentsRepository(TiaraDbContext context, ILogger<PaymentsRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    public async Task<bool> CreatePaymentAsync(Payment payment)
    {
        try
        {
            await _context.Payments.AddAsync(payment);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payment");
            Console.WriteLine(ex.Message);
            return false;
        }
    }
    public async Task<Payment?> GetPaymentByIdAsync(int paymentId)
    {
        try
        {
            return await _context.Payments.FindAsync(paymentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payment by ID");
            // Log the exception
            Console.WriteLine(ex.Message);
            return null;
        }
    }
    public async Task<Payment?> GetPaymentsByTransactionIdAsync(int transactionId)
    {
        try
        {
            return await _context.Payments
                .Where(p => p.TransactionId == transactionId.ToString()).FirstOrDefaultAsync();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payments by transaction ID");
            Console.WriteLine(ex.Message);
            return new Payment();
        }
    }

    public async Task<List<Payment?>?> GetPayments()
    {
        try
        {
            var payments = await _context.Payments.ToListAsync();
            return payments;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all payments");
            return new List<Payment>();

        }
    }
    public async Task UpdatePaymentStatusAsync(int paymentId, string status)
    {
        try
        {
            var payment = await GetPaymentsByTransactionIdAsync(paymentId);
            if (payment == null)
            {
                var ex = new Exception($"Payment with ID {paymentId} not found.");
                _logger.LogError(ex, "Payment not found for status update");
                throw ex;
            }
            payment.PaymentStatus = status;
            _context.Payments.Update(payment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating payment status");
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task<bool> DeletePaymentAsync(int paymentId)
    {
        try
        {
            var payment = await GetPaymentByIdAsync(paymentId);
            if (payment == null) return false;
            _context.Payments.Remove(payment);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting payment");
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public async Task<bool> UpdatePaymentAsync(Payment payment)
    {
        try
        {
            var existingPayment = await GetPaymentByIdAsync(payment.Id);
            if (existingPayment == null) return false;
            existingPayment.PaymentStatus = payment.PaymentStatus;
            existingPayment.Amount = payment.Amount;
            existingPayment.TransactionId = payment.TransactionId;
            _context.Payments.Update(existingPayment);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating payment");
            Console.WriteLine(ex.Message);
            return false;
        }
    }
}
