namespace GalaxyCinemaBackEnd.Models.Request
{
    public class AddMovieRequest
    {
        public IFormFile PosterImage { get; set; }
        public string Title { get; set; }
        public string Director { get; set; }
        public string Synopsis { get; set; }
        public string Duration { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Casts { get; set; }
        public string Writer { get; set; }
        public string Rating { get; set; }
    }
}
