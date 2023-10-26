using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FlightBooking.Models
{
    public class Airport
    {
        public int Id{ get; set; }
        public string name { get; set; }
        public string iata { get; set; }
        public string icao { get; set; }
        public string city { get; set; }
        public double lat { get; set; }
        public double lon { get; set; }
        public string country { get; set; }

        public List<Schedule> DepartureSchedules { get; set; }
        public List<Schedule> DestinationSchedules { get; set; }
    }
}