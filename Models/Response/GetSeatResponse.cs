namespace GalaxyCinemaBackEnd.Models.Response
{
    public class GetSeatResponse
    {
        public List<RowSeatDetail> Data { get; set; }
    }

    public class RowSeatDetail
    {
        public string Row { get; set; }
        public List<SeatDetail> SeatDetail { get; set; }
    }

    public class SeatDetail
    {
        public int Id { get; set; }
        public string SeatName { get; set; }
        public bool IsAvail { get; set; }
    }

}
