namespace GalaxyCinemaBackEnd.Models.Response
{
    public class GetBookingHistoryResponse
    {
        public int BookingHeaderId { get; set; }
        public string StudioName { get; set; }
        public string MovieTitle { get; set; }
        public string Poster { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime ScheduleTimeStart { get; set; }
    }
}
