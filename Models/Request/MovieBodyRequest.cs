namespace GalaxyCinemaBackEnd.Models.Request
{
    public class MovieBodyRequest
    {
        public string Title { get; set; }
        public string Director { get; set; }
        public string Poster { get; set; }
        public string Synopsis { get; set; }
        public int Duration { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Casts { get; set; }
        public string Writer { get; set; }
        public string Rating { get; set; }

    }
}
