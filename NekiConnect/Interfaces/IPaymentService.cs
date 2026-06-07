using Stripe.Checkout;

namespace NekiConnect.Interfaces
{
    public interface IPaymentService
    {
        Task<string> CreateCheckoutSessionAsync(
            decimal amount,
            string campaignTitle,
            int? campaignId,
            string donorId,
            string successUrl,
            string cancelUrl);
        Task<Session> GetSessionAsync(string sessionId);
    }
}