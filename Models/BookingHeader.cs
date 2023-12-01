using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GalaxyCinemaBackEnd.Models
{
    public class BookingHeader
    {
        [Key]
        public int BookingHeaderID { get; set; }
        [Required]

        public String UserID { get; set; }
        [Required]

        public int ScheduleID { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

    }
}
