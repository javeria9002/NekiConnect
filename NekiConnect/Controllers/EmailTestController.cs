using Microsoft.AspNetCore.Mvc;
using NekiConnect.Services;

namespace NekiConnect.Controllers
{
    [ApiController]
    [Route("api/email-test")]
    public class EmailTestController : ControllerBase
    {
        private readonly EmailService _emailService;

        public EmailTestController(EmailService emailService)
        {
            _emailService = emailService;
        }

        // GET /api/email-test?to=your@email.com
        [HttpGet]
        public async Task<IActionResult> Test([FromQuery] string to)
        {
            if (string.IsNullOrEmpty(to))
                return BadRequest(new { message = "Pass ?to=email@example.com in the URL" });

            var ok = await _emailService.SendAsync(
                toEmail: to,
                toName: "Test User",
                subject: "Hello from NekiConnect!",
                htmlContent: "<h2>It works! 🎉</h2><p>Your email service is configured correctly.</p>");

            return ok
                ? Ok(new { message = $"Email sent to {to}" })
                : StatusCode(500, new { message = "Email failed to send. Check server logs." });
        }
    }
}