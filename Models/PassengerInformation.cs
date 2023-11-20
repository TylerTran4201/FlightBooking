using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightBooking.Models
{
    public class PassengerInformation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string FullName { get; set; }
        public string IdCard { get; set; }
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }
    }
}