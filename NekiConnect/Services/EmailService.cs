using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;
using SystemTask = System.Threading.Tasks.Task;

namespace NekiConnect.Services
{
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

        public async System.Threading.Tasks.Task<bool> SendAsync(string toEmail, string toName, string subject, string htmlContent)
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

        public System.Threading.Tasks.Task<bool> SendWelcomeEmailAsync(string toEmail, string toName)
        {
            var html = $@"
                <h2>Welcome to NekiConnect, {toName}! 💚</h2>
                <p>Thank you for joining Pakistan's first online NGO hub.</p>
                <p>You can now donate to verified causes or volunteer with NGOs across the country.</p>
                <hr/>
                <p style='font-size:12px;color:#9ca3af'>NekiConnect — Connecting hearts to causes.</p>";

            return SendAsync(toEmail, toName, "Welcome to NekiConnect!", html);
        }

        // NGO-specific registration email (different look + wording than the donor welcome)
        public System.Threading.Tasks.Task<bool> SendNgoRegistrationPendingEmailAsync(string toEmail, string ngoName)
        {
            var html = $@"
                <div style='font-family:Segoe UI,sans-serif;max-width:600px;margin:auto;padding:32px;'>
                    <h2 style='color:#16a34a;'>Thank you for registering, {ngoName}! 🏢</h2>
                    <p style='color:#374151;'>
                        Your NGO registration has been <strong>received successfully</strong> and is now
                        <strong>pending review</strong> by the NekiConnect admin team.
                    </p>

                    <div style='background:#fffbeb;border:1px solid #fde68a;border-radius:12px;padding:16px 20px;margin:20px 0;'>
                        <p style='margin:0;color:#92400e;font-weight:600;'>⏳ Status: Pending Approval</p>
                        <p style='margin:8px 0 0;color:#92400e;font-size:13px;'>
                            Our team will verify your documents and details. This usually takes 1–2 business days.
                        </p>
                    </div>

                    <p style='color:#374151;'>What happens next:</p>
                    <ul style='color:#4b5563;font-size:14px;line-height:1.7;'>
                        <li>We review your registration and uploaded documents.</li>
                        <li>You'll receive an email once your NGO is <strong>approved</strong>.</li>
                        <li>After approval, you can log in and start creating campaigns &amp; events.</li>
                    </ul>

                    <p style='color:#6b7280;font-size:13px;'>Thank you for choosing to make a difference. 💚</p>
                    <hr/>
                    <p style='font-size:12px;color:#9ca3af'>NekiConnect — Connecting hearts to causes.</p>
                </div>";

            return SendAsync(toEmail, ngoName, "NGO Registration Received — Pending Review", html);
        }

        public System.Threading.Tasks.Task<bool> SendDonationReceiptAsync(string toEmail, string toName, decimal amount, string targetTitle)
        {
            var html = $@"
                <div style='font-family:Segoe UI,sans-serif;max-width:600px;margin:auto;padding:32px;'>
                    <h2 style='color:#16a34a;'>Thank you, {toName}! 💚</h2>
                    <p style='color:#374151;'>Your donation has been received successfully.</p>
                    <div style='background:#f9fafb;border:1px solid #e5e7eb;border-radius:12px;padding:20px;margin:20px 0;'>
                        <table style='width:100%;'>
                            <tr><td style='color:#6b7280;padding:6px 0;'>Cause</td><td style='font-weight:600;color:#111827;'>{targetTitle}</td></tr>
                            <tr><td style='color:#6b7280;padding:6px 0;'>Amount</td><td style='font-weight:700;color:#16a34a;font-size:18px;'>PKR {amount:N0}</td></tr>
                            <tr><td style='color:#6b7280;padding:6px 0;'>Date</td><td style='color:#111827;'>{DateTime.Now:MMM d, yyyy}</td></tr>
                        </table>
                    </div>
                    <p style='color:#6b7280;font-size:13px;'>Your support makes a real difference. ❤️</p>
                    <hr/>
                    <p style='font-size:12px;color:#9ca3af'>NekiConnect — Connecting hearts to causes.</p>
                </div>";

            return SendAsync(toEmail, toName, "Donation Receipt — NekiConnect", html);
        }

        public System.Threading.Tasks.Task<bool> SendNgoApprovedEmailAsync(string toEmail, string ngoName)
        {
            var html = $@"
                <h2>Congratulations! 🎉</h2>
                <p>Your NGO <strong>{ngoName}</strong> has been approved on NekiConnect.</p>
                <p>You can now log in and start creating campaigns.</p>
                <hr/>
                <p style='font-size:12px;color:#9ca3af'>NekiConnect Admin Team</p>";

            return SendAsync(toEmail, ngoName, "Your NGO has been approved!", html);
        }

        public System.Threading.Tasks.Task<bool> SendNgoRejectedEmailAsync(string toEmail, string ngoName, string reason)
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

        public System.Threading.Tasks.Task<bool> SendVolunteerStatusAsync(string toEmail, string toName, string campaignTitle, bool accepted)
        {
            string subject, html;
            if (accepted)
            {
                subject = "Your volunteer application was accepted!";
                html = $@"
                    <div style='font-family:Segoe UI,sans-serif;max-width:600px;margin:auto;padding:32px;'>
                        <h2 style='color:#16a34a;'>Congratulations, {toName}! 🎉</h2>
                        <p>You have been accepted as a volunteer for <strong>{campaignTitle}</strong>.</p>
                        <hr/>
                        <p style='font-size:12px;color:#9ca3af'>NekiConnect</p>
                    </div>";
            }
            else
            {
                subject = "Volunteer Application Update";
                html = $@"
                    <div style='font-family:Segoe UI,sans-serif;max-width:600px;margin:auto;padding:32px;'>
                        <h2>Hi {toName},</h2>
                        <p>Your application for <strong>{campaignTitle}</strong> was not selected this time.</p>
                        <p>Don't give up — there are many other opportunities!</p>
                        <hr/>
                        <p style='font-size:12px;color:#9ca3af'>NekiConnect</p>
                    </div>";
            }

            return SendAsync(toEmail, toName, subject, html);
        }
    }
}