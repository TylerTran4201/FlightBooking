using System.ComponentModel.DataAnnotations;

namespace FlightBooking.Models
{
    public class Airline
    {
        public int Id { get; set; }
        [Display(Name = "Airline")]
        public string Name { get; set; }
        public int AirlineComanyId { get; set; }
        public int ScheduleId { get; set; }
        public double BasePrice { get; set; }
        public Schedule Schedule { get; set; }
        public AirlineCompany AirlineCompany { get; set; } = null;
        public List<Seat> Seats { get; set; } = new List<Seat>();
        public bool Status { get; set; } = false;
    }
}