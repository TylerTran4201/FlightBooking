using System.ComponentModel.DataAnnotations.Schema;

namespace FlightBooking.Models
{
    [Table("Photos")]
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; } = "null";
        public string PublicId { get; set; } = "null";
    }
}
