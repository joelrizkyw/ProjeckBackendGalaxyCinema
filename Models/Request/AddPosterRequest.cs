namespace GalaxyCinemaBackEnd.Models.Request
{
    public class AddPosterRequest
    {
        public string PosterName { get; set; }
        public IFormFile Poster { get; set; }
    }
}
