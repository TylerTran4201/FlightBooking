namespace FlightBooking.Models
{
    public class Bill
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public double TotalPrice { get; set; }
        public Booking Booking { get; set; }
    }
}