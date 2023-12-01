using System.ComponentModel.DataAnnotations;

namespace GalaxyCinemaBackEnd.Models.GalaxyCinemaDB
{
    public class Theater
    {
        [Key]
        public int TheaterID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }

    }
}
