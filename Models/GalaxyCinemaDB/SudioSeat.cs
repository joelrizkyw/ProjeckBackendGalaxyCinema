using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GalaxyCinemaBackEnd.Models.GalaxyCinemaDB
{
    public class StudioSeat
    {
        [Key]
        public int StudioSeatID { get; set; }
        [Required]

        public int StudioID { get; set; }

        [Required]
        [StringLength(10)]
        public string Name { get; set; }

    }
}
