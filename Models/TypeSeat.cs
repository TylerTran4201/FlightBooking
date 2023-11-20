using System.ComponentModel.DataAnnotations;

namespace FlightBooking.Models
{
    public class TypeSeat
    {
        public int Id { get; set; }
         [Display(Name = "Seat Class")]
        public string Name { get; set; }
        public double Price { get; set; }
        public List<Seat> Seats { get; set; }
    }
}
