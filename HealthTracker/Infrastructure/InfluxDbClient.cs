using Microsoft.Extensions.Configuration;
using System;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracker.Infrastructure
{
    public class InfluxDbClient
    {
        private string _uri;
        private string _databaseName;
        private IConfiguration Configuration;

        private static InfluxDbClient instance;
        public void SetConfiguration(IConfiguration configuration)
        {
            Configuration = configuration;

            var influxSection = Configuration.GetSection("InfluxDb");

            _uri = influxSection.GetSection("Uri").Value;
            _databaseName = influxSection.GetSection("DatabaseName").Value;
        }
        private InfluxDbClient()
        {

        }
        public static InfluxDbClient GetInstance()
        {
            if (instance == null)
                instance = new InfluxDbClient();
            return instance;
        }
        public async Task SendAsync(string data)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                client.BaseAddress = new Uri(_uri);

                var content = new System.Net.Http.StringContent(data, Encoding.UTF8, "application/json");

                await client.PostAsync($"/write?db={_databaseName}&precision=s", content);
            }
        }
        public async Task<string> GetAsync(string query)
        {
            string resultContent;

            using (var client = new System.Net.Http.HttpClient())
            {
                client.BaseAddress = new Uri(_uri);

                var responseMessage = await client.GetAsync(query + $"&db={_databaseName}");
                resultContent = await responseMessage.Content.ReadAsStringAsync();
            }

            return resultContent;
        }
    }
}
