using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GalaxyCinemaBackEnd.Models
{
    public class Studio
    {
        [Key]
        public int StudioID { get; set; }
        [Required]

        public int TheaterID { get; set; }
        [Required]

        public int StudioTypeID { get; set; }
        [Required]

        public string Name { get; set; }
    }
}
