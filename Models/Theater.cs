using System.ComponentModel.DataAnnotations;

namespace GalaxyCinemaBackEnd.Models
{
    public class Theater
    {
        [Key]
        public int TheaterID { get; set; }
        [Required]
        public String Name { get; set; }
        [Required]
        public String Address { get; set; }
      
    }
}
