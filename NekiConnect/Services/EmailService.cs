using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;

namespace NekiConnect.Services
{
    // Sends transactional emails via Brevo
    public class EmailService
    {
        private readonly IConfiguration _config;
        private readonly TransactionalEmailsApi _api;

        public EmailService(IConfiguration config)
        {
            _config = config;
            Configuration.Default.ApiKey["api-key"] = _config["Brevo:ApiKey"];
            _api = new TransactionalEmailsApi();
        }

        public async Task<bool> SendAsync(string toEmail, string toName, string subject, string htmlContent)
        {
            try
            {
                var sender = new SendSmtpEmailSender(
                    _config["Brevo:SenderName"],
                    _config["Brevo:SenderEmail"]);

                var receivers = new List<SendSmtpEmailTo>
                {
                    new SendSmtpEmailTo(toEmail, toName)
                };

                var email = new SendSmtpEmail(
                    sender: sender,
                    to: receivers,
                    subject: subject,
                    htmlContent: htmlContent);

                await _api.SendTransacEmailAsync(email);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email send failed: {ex.Message}");
                return false;
            }
        }

        public Task<bool> SendWelcomeEmailAsync(string toEmail, string toName)
        {
            var html = $@"
                <h2>Welcome to NekiConnect, {toName}! 💚</h2>
                <p>Thank you for joining Pakistan's first online NGO hub.</p>
                <p>You can now donate to verified causes or volunteer with NGOs across the country.</p>
                <hr/>
                <p style='font-size:12px;color:#9ca3af'>NekiConnect — Connecting hearts to causes.</p>";

            return SendAsync(toEmail, toName, "Welcome to NekiConnect!", html);
        }

        public Task<bool> SendDonationReceiptAsync(string toEmail, string toName, decimal amount, string targetTitle)
        {
            var html = $@"
                <h2>Thank you, {toName}! 🙏</h2>
                <p>Your generous donation has been received.</p>
                <table style='border-collapse:collapse'>
                    <tr><td style='padding:8px'><strong>Amount:</strong></td><td style='padding:8px'>PKR {amount:N0}</td></tr>
                    <tr><td style='padding:8px'><strong>Cause:</strong></td><td style='padding:8px'>{targetTitle}</td></tr>
                    <tr><td style='padding:8px'><strong>Date:</strong></td><td style='padding:8px'>{DateTime.Now:MMM d, yyyy}</td></tr>
                </table>
                <p>Your support makes a real difference. ❤️</p>
                <hr/>
                <p style='font-size:12px;color:#9ca3af'>NekiConnect — Connecting hearts to causes.</p>";

            return SendAsync(toEmail, toName, "Donation Receipt — NekiConnect", html);
        }

        public Task<bool> SendNgoApprovedEmailAsync(string toEmail, string ngoName)
        {
            var html = $@"
                <h2>Congratulations! 🎉</h2>
                <p>Your NGO <strong>{ngoName}</strong> has been approved on NekiConnect.</p>
                <p>You can now log in and start creating campaigns.</p>
                <hr/>
                <p style='font-size:12px;color:#9ca3af'>NekiConnect Admin Team</p>";

            return SendAsync(toEmail, ngoName, "Your NGO has been approved!", html);
        }

        public Task<bool> SendNgoRejectedEmailAsync(string toEmail, string ngoName, string reason)
        {
            var html = $@"
                <h2>Registration Update</h2>
                <p>Hello {ngoName},</p>
                <p>Your NGO registration could not be approved at this time.</p>
                <p><strong>Reason:</strong> {reason}</p>
                <hr/>
                <p style='font-size:12px;color:#9ca3af'>NekiConnect Admin Team</p>";

            return SendAsync(toEmail, ngoName, "NGO Registration Update", html);
        }

        public Task<bool> SendVolunteerStatusAsync(string toEmail, string toName, string campaignTitle, bool accepted)
        {
            string subject, html;
            if (accepted)
            {
                subject = "🎉 Your volunteer application was accepted!";
                html = $@"
                    <h2>Congratulations, {toName}! 🎉</h2>
                    <p>You've been accepted as a volunteer for <strong>{campaignTitle}</strong>.</p>
                    <hr/>
                    <p style='font-size:12px;color:#9ca3af'>NekiConnect</p>";
            }
            else
            {
                subject = "Volunteer Application Update";
                html = $@"
                    <h2>Hi {toName},</h2>
                    <p>Your application for <strong>{campaignTitle}</strong> was not selected.</p>
                    <p>Don't give up — there are many other opportunities!</p>
                    <hr/>
                    <p style='font-size:12px;color:#9ca3af'>NekiConnect</p>";
            }

            return SendAsync(toEmail, toName, subject, html);
        }
    }
}