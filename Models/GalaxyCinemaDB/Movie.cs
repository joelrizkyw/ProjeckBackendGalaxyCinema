using System.ComponentModel.DataAnnotations;

namespace GalaxyCinemaBackEnd.Models.GalaxyCinemaDB
{
    public class Movie
    {
        [Key]
        public int MovieID { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Director { get; set; }

        [Required]
        public string Poster { get; set; }

        [Required]
        public string Synopsis { get; set; }

        public int? Duration { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        [Required]
        public string Casts { get; set; }

        [Required]
        public string Writer { get; set; }


        [RegularExpression("^(SU|13\\+|17\\+|21\\+)$", ErrorMessage = "Invalid rating.")]
        public string Rating { get; set; }
    }
}
