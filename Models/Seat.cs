using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FlightBooking.Models
{
    public class Seat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int AirlineId { get; set; }
        public int TypeSeatId { get; set; }
        public int Status { get; set; } = 0;
        public Airline Airline { get; set; } = null;
        public TypeSeat TypeSeat { get; set; } = null;
    }
}
