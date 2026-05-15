namespace NekiConnect.Models
{
    public class NGO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Mission { get; set; }
        public string City { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}