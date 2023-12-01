using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GalaxyCinemaBackEnd.Models
{
    public class Schedule
    {
        [Key]
        public int ScheduleID { get; set; }
        [Required]

        public int MovieID { get; set; }

        [Required]
        public int StudioID { get; set; }

        [Required]
        public DateTime TimeStart { get; set; }

    }
}
