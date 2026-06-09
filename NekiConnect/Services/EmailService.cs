using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;
using NekiConnect.Interfaces;

namespace NekiConnect.Services
{
    public class EmailService : IEmailService 
    {
        private readonly IConfiguration _config;
        private readonly TransactionalEmailsApi _api;

        public EmailService(IConfiguration config)
        {
            _config = config;
            Configuration.Default.ApiKey["api-key"] = _config["Brevo:ApiKey"];
            _api = new TransactionalEmailsApi();
        }

        // ── Core sender ──
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

        // ── Welcome email ──
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

        // ── NGO registration PENDING (feature/javeria added this - KEEP IT) ──
        public Task<bool> SendNgoRegistrationPendingEmailAsync(string toEmail, string ngoName)
        {
            var html = $@"
                <div style='font-family:Segoe UI,sans-serif;max-width:600px;margin:auto;padding:32px;'>
                    <h2 style='color:#16a34a;'>Thank you for registering, {ngoName}! 🏢</h2>
                    <p>Your NGO registration has been <strong>received successfully</strong> and is now
                    <strong>pending review</strong>.</p>

                    <div style='background:#fffbeb;border:1px solid #fde68a;border-radius:12px;padding:16px;margin:20px 0;'>
                        <p style='margin:0;font-weight:600;color:#92400e;'>⏳ Status: Pending Approval</p>
                        <p style='margin-top:8px;font-size:13px;color:#92400e;'>
                            Approval usually takes 1–2 business days.
                        </p>
                    </div>

                    <p style='color:#6b7280;font-size:13px;'>NekiConnect — Connecting hearts to causes.</p>
                </div>";

            return SendAsync(toEmail, ngoName, "NGO Registration Received", html);
        }

        // ── Donation receipt ──
        public Task<bool> SendDonationReceiptAsync(string toEmail, string toName, decimal amount, string targetTitle)
        {
            var html = $@"
                <div style='font-family:Segoe UI,sans-serif;max-width:600px;margin:auto;padding:32px;'>
                    <h2 style='color:#16a34a;'>Thank you, {toName}! 💚</h2>
                    <p>Your donation has been received successfully.</p>

                    <table style='width:100%;margin:20px 0;'>
                        <tr><td>Cause</td><td>{targetTitle}</td></tr>
                        <tr><td>Amount</td><td><b>PKR {amount:N0}</b></td></tr>
                        <tr><td>Date</td><td>{DateTime.Now:MMM d, yyyy}</td></tr>
                    </table>

                    <p style='font-size:13px;'>Your support makes a difference ❤️</p>
                </div>";

            return SendAsync(toEmail, toName, "Donation Receipt", html);
        }

        // ── NGO approved ──
        public Task<bool> SendNgoApprovedEmailAsync(string toEmail, string ngoName)
        {
            var html = $@"
                <h2>Congratulations 🎉</h2>
                <p>Your NGO <b>{ngoName}</b> has been approved.</p>";

            return SendAsync(toEmail, ngoName, "NGO Approved", html);
        }

        // ── NGO rejected ──
        public Task<bool> SendNgoRejectedEmailAsync(string toEmail, string ngoName, string reason)
        {
            var html = $@"
                <h2>Registration Update</h2>
                <p>Hello {ngoName}, your NGO was not approved.</p>
                <p><b>Reason:</b> {reason}</p>";

            return SendAsync(toEmail, ngoName, "NGO Registration Update", html);
        }

        public Task<bool> SendNgoSuspendedEmailAsync(string toEmail, string ngoName, string reason)
        {
            var html = $@"
        <div style='font-family:Segoe UI,sans-serif;max-width:600px;margin:auto;padding:32px;'>
            <h2 style='color:#dc2626;'>NGO Account Suspended ⚠</h2>
            <p>Dear <strong>{ngoName}</strong>,</p>
            <p>Your NGO account on NekiConnect has been <strong>suspended</strong>.</p>

            <div style='background:#fef2f2;border:1px solid #fecaca;border-radius:12px;padding:16px;margin:20px 0;'>
                <p style='margin:0;font-weight:600;color:#dc2626;'>Reason for Suspension:</p>
                <p style='margin-top:8px;color:#7f1d1d;'>{reason}</p>
            </div>

            <p>If you believe this is a mistake, please contact our support team.</p>
            <p style='color:#6b7280;font-size:13px;'>NekiConnect — Connecting hearts to causes.</p>
        </div>";

            return SendAsync(toEmail, ngoName, "NGO Account Suspended — NekiConnect", html);
        }

        // ── Volunteer status ──
        public Task<bool> SendVolunteerStatusAsync(string toEmail, string toName, string campaignTitle, bool accepted)
        {
            string subject;
            string html;

            if (accepted)
            {
                subject = "Volunteer Accepted 🎉";
                html = $@"
                    <h2>Congratulations {toName}!</h2>
                    <p>You are selected for <b>{campaignTitle}</b>.</p>";
            }
            else
            {
                subject = "Volunteer Application Update";
                html = $@"
                    <h2>Hi {toName}</h2>
                    <p>Not selected for <b>{campaignTitle}</b>.</p>";
            }

            return SendAsync(toEmail, toName, subject, html);
        }
    }
}