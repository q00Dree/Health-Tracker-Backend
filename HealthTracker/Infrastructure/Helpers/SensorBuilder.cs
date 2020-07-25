using HealthTracker.Models;

namespace HealthTracker.Infrastructure.Helpers
{
    public class SensorBuilder
    {
        public static Sensor Build(string deviceId, string sensorType)
        {
            switch (sensorType)
            {
                case "location":
                    return new LocationSensor(deviceId, sensorType);
                case "temperature":
                    return new TemperatureSensor(deviceId, sensorType);
                case "fall":
                    return new FallSensor(deviceId, sensorType);
                case "sos":
                    return new SosSensor(deviceId, sensorType);
                default:
                    return null;
            }
        }

    }
}
