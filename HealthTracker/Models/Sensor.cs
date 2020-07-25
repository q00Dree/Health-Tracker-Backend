using Qoollo.Infrastructure;
using System;
using System.Threading.Tasks;

namespace HealthTracker.Models
{
    public abstract class Sensor
    {
        public int PostgreSQLId { get; set; }
        public string DeviceId { get; set; }
        public string SensorType { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool AlertState { get; set; }

        protected object currentValue;
        protected InfluxDbClient influxDbClient;
        public Sensor(string deviceId, string sensorType)
        {
            DeviceId = deviceId;
            SensorType = sensorType;

            influxDbClient = InfluxDbClient.GetInstance();
        }
        public Sensor() { }
        public abstract Task Save();
        protected abstract bool Handle(object data);
        public abstract Task<object> GetValue(int count);
        public abstract void SetValue(object value);
    }
}
