namespace GalaxyCinemaBackEnd.Models.Request
{
    public class AddPaymentRequest
    {
        public int BookingHeaderID { get; set; }
        public int ScheduleID { get; set; }
        public int SeatQty { get; set; }
    }
}
