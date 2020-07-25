using Qoollo.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace HealthTracker.Models
{
    public class LocationSensor : Sensor
    {
        public LocationSensor(string deviceId, string sensorType) : base(deviceId, sensorType) { }
        public LocationSensor() { }
        protected override bool Handle(object data)
        {
            return false;
        }
        public override void SetValue(object value)
        {
            JsonElement json = (JsonElement)value;
            var doubleValue = JsonSerializer.Deserialize<double[]>((json.GetRawText()));

            if (doubleValue is double[] points)
            {
                foreach (var item in points)
                {
                    if (item < -90.0 || item > 90.0)
                    {
                        throw new Exception(message: $"DeviceId: {DeviceId}, SensorType: {SensorType}, point {item} cant be more than 90.0 and less than -90");
                    }
                }

                currentValue = points;

                AlertState = Handle(currentValue);
            }
            else
            {
                throw new Exception(message: $"DeviceId: {DeviceId}, SensorType: {SensorType}, currentValue is not double[]: {currentValue}");
            }
        }
        public override async Task Save()
        {
            var points = currentValue as double[];

            //Example sensors_location,postgre_id=1 latitude=34.43,longitude=45.42 12121212112
            var influxPostData =
                $"sensors_{SensorType}," +
                $"postgre_id={PostgreSQLId} " +
                $"latitude={points[0].ToString().Replace(',', '.')}," +
                $"longitude={points[1].ToString().Replace(',', '.')} " +
                $"{TimeStamp.ToUnixTime()}";

            await influxDbClient.SendAsync(influxPostData);
        }
        public override async Task<object> GetValue(int count)
        {
            string query =
                $"/query?q=SELECT \"latitude\",\"longitude\" FROM sensors_{SensorType} " +
                $"WHERE postgre_id=\'{PostgreSQLId}\' " +
                $"ORDER BY \"time\" DESC " +
                $"LIMIT {count}";

            string json = await influxDbClient.GetAsync(query);

            List<List<object>> resultCollection = InfluxDbJsonSerializer.Deserialize(json);

            if (resultCollection?.Count() > --count)
            {
                TimeStamp = DateTime.Parse(resultCollection[count][0].ToString()).ToUniversalTime();

                JsonElement latJson = (JsonElement)resultCollection[count][1];
                JsonElement lonJson = (JsonElement)resultCollection[count][2];

                double latitude = JsonSerializer.Deserialize<double>(latJson.GetRawText());
                double longitude = JsonSerializer.Deserialize<double>(lonJson.GetRawText());

                double[] location = new double[] { latitude, longitude };

                AlertState = Handle(location);

                return location;
            }
            else
                return null;
        }
    }
}


