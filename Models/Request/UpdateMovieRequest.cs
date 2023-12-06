namespace GalaxyCinemaBackEnd.Models.Request
{
    public class UpdateMovieRequest
    {
        public IFormFile? NewPoster { get; set; }
        public string MovieID { get; set; } 
        public string NewTitle { get; set; }
        public string OldTitle { get; set; }
        public string Director { get; set; }
        public string Synopsis { get; set; }
        public string Duration { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Casts { get; set; }
        public string OldPoster { get; set; }
        public string Writer { get; set; }
        public string Rating { get; set; }
    }
}
