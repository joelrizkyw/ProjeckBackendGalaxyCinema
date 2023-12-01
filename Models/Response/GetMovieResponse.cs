namespace GalaxyCinemaBackEnd.Models.Response
{
    public class GetMovieResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Director { get; set; }
        public string Poster { get; set; }
        public string Synopsis { get; set; }
        public int Duration { get; set; }
        public Boolean IsPlaying { get; set; }
        public string Casts { get; set; }
        public string Writer { get; set; }

    }
}
