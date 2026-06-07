using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NekiConnect.Models;
using NekiConnect.Interfaces;
using System.Security.Claims;

namespace NekiConnect.Controllers
{
    [ApiController]
    [Route("api/donations")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DonationApiController : ControllerBase
    {
        private readonly IDonationService _donationService;
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICampaignService _campaignService;
        private readonly IPaymentService _paymentService;

        public DonationApiController(
            IDonationService donationService,
            INotificationService notificationService,
            IEmailService emailService,
            UserManager<ApplicationUser> userManager,
            ICampaignService campaignService,
            IPaymentService paymentService)
        {
            _donationService = donationService;
            _notificationService = notificationService;
            _emailService = emailService;
            _userManager = userManager;
            _campaignService = campaignService;
            _paymentService = paymentService;
        }

        [HttpGet("mine")]
        public async Task<IActionResult> MyDonations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue("sub");
            if (userId == null) return Unauthorized();

            var donations = await _donationService.GetDonationsByDonorIdAsync(userId);
            return Ok(donations.Select(d => new
            {
                d.Id,
                d.Amount,
                d.PaymentMethod,
                d.PaymentReference,
                d.DonatedAt,
                Campaign = d.Campaign == null ? null : new
                {
                    d.Campaign.Id,
                    d.Campaign.Title,
                    Ngo = d.Campaign.NGO?.Name
                }
            }));
        }

        [HttpPost("manual")]
        public async Task<IActionResult> ManualDonate([FromBody] ManualDonationRequest req)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue("sub");
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            string campaignTitle = "General Donation";
            if (req.CampaignId.HasValue)
            {
                var campaign = await _campaignService.GetCampaignByIdAsync(req.CampaignId.Value);
                if (campaign != null) campaignTitle = campaign.Title;
            }

            var donation = new Donation
            {
                DonorId = userId,
                CampaignId = req.CampaignId,
                Amount = req.Amount,
                PaymentMethod = req.PaymentMethod,
                PaymentReference = req.PaymentReference
            };

            await _donationService.AddDonationAsync(donation);
            await _notificationService.NotifyDonationReceiptAsync(userId, req.Amount, campaignTitle);

            if (user.Email != null)
                await _emailService.SendDonationReceiptAsync(user.Email, user.FullName, req.Amount, campaignTitle);

            return Ok(new { message = $"Donation of PKR {req.Amount:N0} to '{campaignTitle}' recorded successfully." });
        }

        [HttpPost("stripe-checkout")]
        public async Task<IActionResult> StripeCheckout([FromBody] StripeCheckoutRequest req)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue("sub");
            if (userId == null) return Unauthorized();

            string campaignTitle = "General Donation";
            if (req.CampaignId.HasValue)
            {
                var campaign = await _campaignService.GetCampaignByIdAsync(req.CampaignId.Value);
                if (campaign != null) campaignTitle = campaign.Title;
            }

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var url = await _paymentService.CreateCheckoutSessionAsync(
                req.Amount,
                campaignTitle,
                req.CampaignId,
                userId,
                $"{baseUrl}/api/donations/stripe-success",
                $"{baseUrl}/campaigns"
            );

            return Ok(new { url });
        }

        [AllowAnonymous]
        [HttpGet("stripe-success")]
        public async Task<IActionResult> StripeSuccess([FromQuery] string session_id)
        {
            try
            {
                var session = await _paymentService.GetSessionAsync(session_id);
                if (session.PaymentStatus != "paid")
                    return Redirect("/donor/donations?error=payment_failed");

                var donorId = session.Metadata["donorId"];
                var campaignTitle = session.Metadata["campaignTitle"];
                int? campaignId = int.TryParse(session.Metadata["campaignId"], out var cid) ? cid : null;

                long amountLong = session.AmountTotal ?? 0;
                decimal amountUsd = amountLong / 100m;

                var user = await _userManager.FindByIdAsync(donorId);
                if (user == null) return Redirect("/donor/donations?error=user_not_found");

                var existing = await _donationService.GetDonationsByDonorIdAsync(donorId);
                bool alreadySaved = existing.Any(d => d.PaymentReference == session_id);

                if (!alreadySaved)
                {
                    var donation = new Donation
                    {
                        DonorId = donorId,
                        CampaignId = campaignId,
                        Amount = amountUsd,
                        PaymentMethod = "Stripe",
                        PaymentReference = session_id
                    };

                    await _donationService.AddDonationAsync(donation);
                    await _notificationService.NotifyDonationReceiptAsync(donorId, amountUsd, campaignTitle);

                    if (user.Email != null)
                        await _emailService.SendDonationReceiptAsync(
                            user.Email, user.FullName, amountUsd, campaignTitle);
                }

                return Redirect("/donor/donations?success=stripe");
            }
            catch
            {
                return Redirect("/donor/donations?error=failed");
            }
        }
    }

    public class ManualDonationRequest
    {
        public int? CampaignId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = "JazzCash";
        public string? PaymentReference { get; set; }
    }

    public class StripeCheckoutRequest
    {
        public int? CampaignId { get; set; }
        public decimal Amount { get; set; }
    }
}