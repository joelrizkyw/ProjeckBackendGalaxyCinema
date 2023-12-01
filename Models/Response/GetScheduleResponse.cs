namespace GalaxyCinemaBackEnd.Models.Response
{
    public class GetScheduleResponse
    {
        public int ScheduleId { get; set; }
        public int MovieId { get; set; }
        public int StudioId { get; set; }
        public int StudioTypeId { get; set; }
        public DateTime TimeStart { get; set; }
        public string StudioTypeName { get; set; }
        public int Price { get; set; }
        public string Rating { get; set; }

    }
}
