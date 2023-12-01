using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GalaxyCinemaBackEnd.Models.GalaxyCinemaDB
{
    public class BookingHeader
    {
        [Key]
        public int BookingHeaderID { get; set; }
        [Required]

        public Guid UserID { get; set; }
        [Required]

        public int ScheduleID { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

    }
}
