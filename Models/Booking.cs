using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightBooking.Models
{
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int UserId { get; set; }
        public bool IsPaid { get; set; } = false;
        public int ScheduleId { get; set; }
        public List<Ticket> Tickets { get; set; } = new List<Ticket>();
        public Schedule Schedule { get; set; }
        public AppUser User { get; set; }
    }
}