using System.ComponentModel.DataAnnotations;

namespace GalaxyCinemaBackEnd.Models
{
    public class StudioType
    {
        [Key]
        public int StudioTypeId { get; set; }
        [Required]
        public String Name { get; set; }
        [Required]
        public String Description { get; set; }
    }
}
