namespace FlightBooking.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public int SeatId { get; set; }
        public int BookingId { get; set; }
        public int PassengerInformationId { get; set; }
        public PassengerInformation PassengerInformation { get; set; }
        public Seat Seat { get; set; }
        public Booking Booking { get; set; }
    }
}