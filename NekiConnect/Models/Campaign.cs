namespace NekiConnect.Models
{
    public class Campaign
    {
        public int Id { get; set; }
        public int NgoId { get; set; }
        public string Title { get; set; }
        public string Story { get; set; }
        public decimal GoalAmount { get; set; }
        public decimal RaisedAmount { get; set; }
        public DateTime Deadline { get; set; }
        public string Status { get; set; } // active, closed
    }
}