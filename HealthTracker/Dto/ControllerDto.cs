using System.ComponentModel.DataAnnotations;

namespace HealthTracker.Dto
{
    public class ControllerDto
    {
        [Required]
        public string DeviceId { get; set; }
        [Required]
        public bool IsOnline { get; set; }
    }
}
