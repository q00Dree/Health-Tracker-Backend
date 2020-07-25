using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace HealthTracker.Infrastructure.Helpers
{
    public class InfluxDbJsonSerializer
    {
        public static List<List<object>> Deserialize(string json)
        {
            List<List<object>> resultCollection = new List<List<object>>();
            Root response = JsonSerializer.Deserialize<Root>(json);

            if (response.results[0].series == null)
                return null;

            foreach (var result in response.results.SelectMany(n => n.series).SelectMany(n => n.values))
            {
                // 0 элемент timestamp
                // 1 элемент currentValue
                // 2 элемент latitude
                // 3 элемент longitude

                resultCollection.Add(result);
            }

            return resultCollection;
        }
    }

    public class Root
    {
        public List<Result> results { get; set; }
    }
    public class Result
    {
        public int statement_id { get; set; }
        public List<Series> series { get; set; }
    }
    public class Series
    {
        public string name { get; set; }
        public List<string> columns { get; set; }
        public List<List<object>> values { get; set; }

    }
}

