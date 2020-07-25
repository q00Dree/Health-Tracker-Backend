using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HealthTracker.DAL.Entities
{
    public class Sensor
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string DeviceId { get; set; }
        [Required]
        public string SensorType { get; set; }

        [ForeignKey("DeviceId")]
        public virtual Controller Controller { get; set; }
    }
}
