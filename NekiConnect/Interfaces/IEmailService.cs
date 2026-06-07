namespace NekiConnect.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendAsync(string toEmail, string toName, string subject, string htmlContent);
        Task<bool> SendWelcomeEmailAsync(string toEmail, string toName);
        Task<bool> SendNgoRegistrationPendingEmailAsync(string toEmail, string ngoName);
        Task<bool> SendDonationReceiptAsync(string toEmail, string toName, decimal amount, string targetTitle);
        Task<bool> SendNgoApprovedEmailAsync(string toEmail, string ngoName);
        Task<bool> SendNgoRejectedEmailAsync(string toEmail, string ngoName, string reason);
        Task<bool> SendVolunteerStatusAsync(string toEmail, string toName, string campaignTitle, bool accepted);
    }
}