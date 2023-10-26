namespace FlightBooking.Models
{
    public class TypeSeat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double  Price { get; set; }
        public List<Seat> Seats { get; set; }
    }
}
