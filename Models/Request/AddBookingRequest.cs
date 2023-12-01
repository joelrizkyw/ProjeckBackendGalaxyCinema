namespace GalaxyCinemaBackEnd.Models.Request
{
    public class AddBookingRequest
    {
        public int ScheduleID { get; set; }
        public Guid userID { get; set; }
        public List<int> seatID{ get; set; }
    }
}
