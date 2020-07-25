using System;
using System.ComponentModel.DataAnnotations;

namespace HealthTracker.Dto
{
    public class GetSensorDto
    {
        [Required]
        public string DeviceId { get; set; }
        [Required]
        public string SensorType { get; set; }
        [Required]
        public object CurrentValue { get; set; }
        [Required]
        public DateTime TimeStamp { get; set; }
        [Required]
        public bool AlertState { get; set; }
    }
}
