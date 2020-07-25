using HealthTracker.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace HealthTracker.Models
{
    public class SosSensor : Sensor
    {
        public SosSensor(string deviceId, string sensorType) : base(deviceId, sensorType) { }
        public SosSensor() { }
        protected override bool Handle(object data)
        {
            return (bool)data ? true : false;
        }
        public override void SetValue(object value)
        {
            JsonElement json = (JsonElement)value;

            var boolValue = JsonSerializer.Deserialize<bool>((json.GetRawText()));

            if (boolValue is bool b)
            {
                currentValue = b;

                AlertState = Handle(currentValue);
            }
            else
            {
                throw new Exception(message: $"DeviceId: {DeviceId}, SensorType: {SensorType}, currentValue is not bool: {currentValue}");
            }
        }
        public override async Task Save()
        {
            //Example sensors_sos,postgre_id=1 currentValue=true 12121212112
            var influxPostData =
                $"sensors_{SensorType}," +
                $"postgre_id={PostgreSQLId} " +
                $"currentValue={currentValue} " +
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

                JsonElement stateJson = (JsonElement)resultCollection[count][1];
                bool state = JsonSerializer.Deserialize<bool>(stateJson.GetRawText());

                AlertState = Handle(state);

                return state;
            }
            else
                return null;
        }
    }
}
