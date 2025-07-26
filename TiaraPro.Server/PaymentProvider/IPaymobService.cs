using TiaraPro.Server.Models;

namespace TiaraPro.Server.PaymentProvider
{
    public interface IPaymobService
    {
        Task<PaymobIntentionResponse> CreateIntentionAsync(PaymobIntentionRequest request);

        //Task<PaymobIntentionResponse> GetTransactionCallbackResponse(string transactionId);
    }
}
