namespace FlightBooking.Models
{
    public class Airline
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AirlineComanyId { get; set; }
        public AirlineCompany AirlineCompany { get; set; } = null;
        public List<Seat> Seats { get; set; }
    }
}