using NekiConnect.Models;

namespace NekiConnect.Models
{
    public class CategoryTotal
    {
        public string Category { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }

    public class UserOverview
    {
        public ApplicationUser User { get; set; } = default!;
        public decimal TotalDonated { get; set; }
        public bool HasVolunteered { get; set; }
    }

    public class NgoOverview
    {
        public NGO NGO { get; set; } = default!;
        public int Campaigns { get; set; }
        public decimal TotalRaised { get; set; }
        public int Volunteers { get; set; }
        public bool IsNew { get; set; }
    }

    public class MonthlyDonation
    {
        public string Month { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }
}