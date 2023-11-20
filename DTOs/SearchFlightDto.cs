namespace FlightBooking.DTOs
{
    public class SearchFlightDto
    {
        public string Departure { get; set; }
        public string Destination { get; set; }   
        public DateOnly FlightDate  { get; set; }     
        public int NumOfTickets { get; set; }
    }
}