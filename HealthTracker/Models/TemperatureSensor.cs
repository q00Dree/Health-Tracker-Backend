using HealthTracker.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace HealthTracker.Models
{
    public class TemperatureSensor : Sensor
    {
        public TemperatureSensor(string deviceId, string sensorType) : base(deviceId, sensorType) { }
        public TemperatureSensor() { }
        protected override bool Handle(object data)
        {
            return false;
        }
        public override void SetValue(object value)
        {
            JsonElement json = (JsonElement)value;
            var floatValue = JsonSerializer.Deserialize<float>((json.GetRawText()));

            if (floatValue is float temp)
            {
                if (temp < 34F || temp > 42F)
                {
                    throw new Exception(message: $"DeviceId: {DeviceId}, SensorType: {SensorType}, temperature {temp} cant be more than 42C and less than 34C");
                }

                currentValue = temp;

                AlertState = Handle(currentValue);
            }
            else
            {
                throw new Exception(message: $"DeviceId: {DeviceId}, SensorType: {SensorType}, currentValue is not float: {currentValue}");
            }

        }

        public override async Task Save()
        {
            //Example sensors_location,postgre_id=1 currentValue="34.43, 45.42" 12121212112
            var influxPostData =
                $"sensors_{SensorType}," +
                $"postgre_id={PostgreSQLId} " +
                $"currentValue={currentValue.ToString().Replace(',', '.')} " +
                $"{TimeStamp.ToUnixTime()}";

            await influxDbClient.SendAsync(influxPostData);
        }
        public override async Task<object> GetValue(int count)
        {
            string query =
                $"/query?q=SELECT \"currentValue\" FROM sensors_{SensorType} " +
                $"WHERE postgre_id=\'{PostgreSQLId}\' " +
                $"ORDER BY \"time\" DESC " +
                $"LIMIT {count}";

            string json = await influxDbClient.GetAsync(query);

            List<List<object>> resultCollection = InfluxDbJsonSerializer.Deserialize(json);

            if (resultCollection?.Count() > --count)
            {
                TimeStamp = DateTime.Parse(resultCollection[count][0].ToString()).ToUniversalTime();

                JsonElement tempJson = (JsonElement)resultCollection[count][1];
                float temperature = JsonSerializer.Deserialize<float>(tempJson.GetRawText());

                AlertState = Handle(temperature);

                return temperature;
            }
            else
                return null;
        }
    }
}

