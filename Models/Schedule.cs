using System.ComponentModel.DataAnnotations;

namespace FlightBooking.Models
{
    public class Schedule
    {
        public int Id { get; set; }
        public int DestinationAirportId { get; set; }
        public int DepartureAirportId { get; set; }
        public TimeSpan FlightTime { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime DestinationTime { get; set; }

        public int AirlineId { get; set; }

        public Airline Airline { get; set; }

        [Display(Name = "Destination Airport")]
        public Airport DestinationAirport { get; set; } = null;

        [Display(Name = "Departure Airport")]
        public Airport DepartureAirport { get; set; } = null;

        public static DateTime GetDestinationTime(DateTime DepartureTime, TimeSpan flightTime)
        {
            return DepartureTime.Add(flightTime);
        }
    }
}