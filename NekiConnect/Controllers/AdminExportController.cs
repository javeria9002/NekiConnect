using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NekiConnect.Services;

namespace NekiConnect.Controllers
{
    [Authorize(Roles = "Admin")]   // cookie auth — only logged-in admins
    [Route("export")]
    public class AdminExportController : Controller
    {
        private readonly AdminService _admin;

        public AdminExportController(AdminService admin)
        {
            _admin = admin;
        }

        [HttpGet("users")]
        public async Task<IActionResult> ExportUsers()
        {
            var rows = await _admin.GetUserOverviewAsync();

            var sb = new StringBuilder();
            sb.AppendLine("Name,Email,Role,Type,Joined,TotalDonated,Status");

            foreach (var r in rows)
            {
                var type = r.User.Role == "NGO" ? "NGO Admin"
                         : r.User.Role == "Admin" ? "Admin"
                         : r.HasVolunteered ? "Volunteer" : "Donor";
                var status = r.User.IsSuspended ? "Suspended" : "Active";

                sb.AppendLine(string.Join(",",
                    Csv(r.User.FullName),
                    Csv(r.User.Email),
                    Csv(r.User.Role),
                    type,
                    r.User.CreatedAt.ToString("yyyy-MM-dd"),
                    r.TotalDonated.ToString("0"),
                    status));
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", $"users_{DateTime.Now:yyyyMMdd}.csv");
        }

        [HttpGet("donations")]
        public async Task<IActionResult> ExportDonations()
        {
            var donations = await _admin.GetAllDonationsAsync();

            var sb = new StringBuilder();
            sb.AppendLine("Donor,Campaign,NGO,Amount,Method,Date");

            foreach (var d in donations)
            {
                sb.AppendLine(string.Join(",",
                    Csv(d.Donor?.FullName),
                    Csv(d.Campaign?.Title),
                    Csv(d.Campaign?.NGO?.Name),
                    d.Amount.ToString("0"),
                    Csv(d.PaymentMethod),
                    d.DonatedAt.ToString("yyyy-MM-dd")));
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", $"donations_{DateTime.Now:yyyyMMdd}.csv");
        }

        [HttpGet("ngos")]
        public async Task<IActionResult> ExportNgos()
        {
            var rows = await _admin.GetNgoOverviewAsync();

            var sb = new StringBuilder();
            sb.AppendLine("Name,City,Category,Status,Campaigns,TotalRaised,Volunteers,Joined");

            foreach (var r in rows)
            {
                sb.AppendLine(string.Join(",",
                    Csv(r.NGO.Name),
                    Csv(r.NGO.City),
                    Csv(r.NGO.Category),
                    Csv(r.NGO.Status),
                    r.Campaigns.ToString(),
                    r.TotalRaised.ToString("0"),
                    r.Volunteers.ToString(),
                    r.NGO.CreatedAt.ToString("yyyy-MM-dd")));
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", $"ngos_{DateTime.Now:yyyyMMdd}.csv");
        }

        // Escapes commas/quotes so the CSV stays valid
        private static string Csv(string? v)
        {
            v ??= "";
            if (v.Contains(',') || v.Contains('"') || v.Contains('\n'))
                return "\"" + v.Replace("\"", "\"\"") + "\"";
            return v;
        }
    }
}