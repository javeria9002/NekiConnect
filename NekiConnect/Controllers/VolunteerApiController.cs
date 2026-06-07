using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NekiConnect.Interfaces;
using NekiConnect.Models;
using System.Security.Claims;

namespace NekiConnect.Controllers
{
    [ApiController]
    [Route("api/volunteer")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class VolunteerApiController : ControllerBase
    {
        private readonly IVolunteerService _volunteerService;
        private readonly INotificationService _notificationService;

        public VolunteerApiController(IVolunteerService volunteerService, INotificationService notificationService)
        {
            _volunteerService = volunteerService;
            _notificationService = notificationService;
        }

        [HttpGet("mine")]
        public async Task<IActionResult> MyApplications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue("sub");
            if (userId == null) return Unauthorized();

            var apps = await _volunteerService.GetApplicationsByUserIdAsync(userId);
            return Ok(apps.Select(a => new
            {
                a.Id,
                a.Skills,
                a.Availability,
                a.Status,
                a.AppliedAt,
                a.Attended,
                Campaign = a.Campaign == null ? null : new { a.Campaign.Id, a.Campaign.Title, Ngo = a.Campaign.NGO?.Name }
            }));
        }

        [HttpPost("apply")]
        public async Task<IActionResult> Apply([FromBody] ApplyRequest req)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue("sub");
            if (userId == null) return Unauthorized();

            var alreadyApplied = await _volunteerService.HasAppliedAsync(userId, req.CampaignId);
            if (alreadyApplied)
                return BadRequest(new { message = "You have already applied for this campaign." });

            var application = new VolunteerApplication
            {
                UserId = userId,
                CampaignId = req.CampaignId,
                Skills = req.Skills,
                Availability = req.Availability
            };

            await _volunteerService.ApplyAsync(application);
            return Ok(new { message = "Application submitted successfully." });
        }
    }

    public class ApplyRequest
    {
        public int CampaignId { get; set; }
        public string Skills { get; set; } = string.Empty;
        public string Availability { get; set; } = string.Empty;
    }
}