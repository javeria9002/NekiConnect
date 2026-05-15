namespace NekiConnect.Models
{
    public class Event
    {
        public int Id { get; set; }
        public int NgoId { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public DateTime EventDate { get; set; }
        public int SeatsAvailable { get; set; }
    }
}