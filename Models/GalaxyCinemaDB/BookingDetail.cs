using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GalaxyCinemaBackEnd.Models.GalaxyCinemaDB
{
    public class BookingDetail
    {
        [Key]
        public int BookingDetailID { get; set; }
        [Required]

        public int BookingHeaderID { get; set; }
        [Required]

        public int StudioSeatID { get; set; }

    }
}
