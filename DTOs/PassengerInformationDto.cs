using System.ComponentModel.DataAnnotations;

namespace FlightBooking.DTOs
{
    public class PassengerInformationDto
    {
        
        [Required]
        public string FullName { get; set; }
        [Required]
        public string IdCard { get; set; }
    }
}