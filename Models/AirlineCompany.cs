using System.ComponentModel.DataAnnotations;

namespace FlightBooking.Models
{
    public class AirlineCompany
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Airline Company")]
        public string Name { get; set; }

        public int PhotoId { get; set; }

        public Photo Photo { get; set; }
        public ICollection<Airline> Airlines { get; set; }
    }
}