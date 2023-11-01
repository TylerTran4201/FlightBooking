namespace FlightBooking.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public bool IsPaid { get; set; }
        public int ScheduleId { get; set; }
        public List<Ticket> Tickets { get; set; } = new List<Ticket>();
        public Schedule Schedule { get; set; }
    }
}