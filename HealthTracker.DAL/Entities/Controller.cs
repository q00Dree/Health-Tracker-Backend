using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HealthTracker.DAL.Entities
{
    public class Controller
    {
        [Key]
        [Required]
        public string DeviceId { get; set; }
        [Required]
        public bool IsOnline { get; set; }
        public virtual List<Sensor> Sensors { get; set; }
        public Controller()
        {
            Sensors = new List<Sensor>();
        }
    }
}
