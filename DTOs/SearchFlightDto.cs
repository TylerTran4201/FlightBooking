using System.ComponentModel.DataAnnotations;

namespace FlightBooking.DTOs
{
    public class SearchFlightDto
    {
        public string Departure { get; set; }
        public string Destination { get; set; }
        [Required]
        public DateOnly FlightDate  { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int NumOfTickets { get; set; }
    }
}