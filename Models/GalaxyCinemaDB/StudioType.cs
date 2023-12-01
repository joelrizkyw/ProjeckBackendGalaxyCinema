using System.ComponentModel.DataAnnotations;

namespace GalaxyCinemaBackEnd.Models.GalaxyCinemaDB
{
    public class StudioType
    {
        [Key]
        public int StudioTypeId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
