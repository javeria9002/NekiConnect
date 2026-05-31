using Stripe;
using Stripe.Checkout;

namespace NekiConnect.Services
{
    public class PaymentService
    {
        private readonly IConfiguration _config;

        public PaymentService(IConfiguration config)
        {
            _config = config;
            StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
        }

        public async Task<string> CreateCheckoutSessionAsync(
            decimal amount,
            string campaignTitle,
            int? campaignId,
            string donorId,
            string successUrl,
            string cancelUrl)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"Donation: {campaignTitle}",
                                Description = "NekiConnect – Pakistan NGO Platform"
                            },
                            UnitAmount = (long)(amount * 100)
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = successUrl + "?session_id={CHECKOUT_SESSION_ID}&campaignId=" + campaignId,
                CancelUrl = cancelUrl,
                Metadata = new Dictionary<string, string>
                {
                    { "donorId", donorId },
                    { "campaignId", campaignId?.ToString() ?? "" },
                    { "campaignTitle", campaignTitle }
                }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);
            return session.Url;
        }

        public async Task<Session> GetSessionAsync(string sessionId)
        {
            var service = new SessionService();
            return await service.GetAsync(sessionId);
        }
    }
}