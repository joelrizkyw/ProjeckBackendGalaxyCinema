using System.ComponentModel.DataAnnotations;

namespace GalaxyCinemaBackEnd.Models
{
    public class Movie
    {
        [Key]
        public int MovieID { get; set; }

        [Required]
        public String Title { get; set; }

        [Required]
        public String Director { get; set; }

        [Required]
        public String Poster { get; set; }

        [Required]
        public String Synopsis { get; set; }

        public int? Duration { get; set; } 

        [Required]
        public DateTime ReleaseDate { get; set; }

        [Required]
        public String Casts { get; set; }

        [Required]
        public String Writer { get; set; }

       
        [RegularExpression("^(SU|13\\+|17\\+|21\\+)$", ErrorMessage = "Invalid rating.")]
        public String Rating { get; set; }
    }
}
