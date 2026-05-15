namespace NekiConnect.Models
{
    public class VolunteerApplication
    {
        public int Id { get; set; }
        public string DonorId { get; set; }
        public int EventId { get; set; }
        public string Skills { get; set; }
        public string Status { get; set; }
        public bool Attended { get; set; }
    }
}