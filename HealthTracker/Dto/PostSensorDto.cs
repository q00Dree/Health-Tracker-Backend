using System;
using System.ComponentModel.DataAnnotations;

namespace HealthTracker.Dto
{
    public class PostSensorDto
    {
        [Required]
        public string SensorType { get; set; }
        [Required]
        public object CurrentValue { get; set; }
        [Required]
        public DateTime TimeStamp { get; set; }
    }
}
