using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Rijnkaai.Abstractions;
using Rijnkaai.Domain;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Rijnkaai.Business
{
    public class RijnkaaiService : IRijnkaaiService
    {
        private readonly HttpClient client;
        private readonly string UUID = "70480b72-0f6d-4823-82d9-f7d900f68f32";
        private readonly ILogger<RijnkaaiService> _logger;
        public RijnkaaiService(IHttpClientFactory clientFactory, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<RijnkaaiService>();
            client = clientFactory.CreateClient();
        }
        public async Task<IEnumerable<ParkingReport>> GetRijnkaaiClosedDates()
        {
            _logger.LogInformation($"Calling Antwerp Rijnkaai Parking Content URL: {Environment.GetEnvironmentVariable("Antwerp-Parking-API-Url")}");

            var response = await client.GetAsync(Environment.GetEnvironmentVariable("Antwerp-Parking-API-Url"));

            var responseContent = JObject.Parse(await response.Content.ReadAsStringAsync());

            _logger.LogInformation($"Looking for entry with UUID: {UUID}");

            var root = responseContent!.SelectToken(".fields.tabs[:1].value.omschrijving")?.FirstOrDefault(z => z.Value<string>("uuid") == UUID)?.Value<string>("value");

            if (root is null)
            {
                return Enumerable.Empty<ParkingReport>();
            }

            var matches = Regex.Matches(root, "<li>([A-Z0-9a-z:\\s]*)<\\/li>");

            var reports = new List<ParkingReport>();

            foreach (Match match in matches)
            {
                var split = match.Groups.Values.Last().Value.Split(":");
                var date = split[0];

                var dateObject = DateTime.Parse(date, CultureInfo.GetCultureInfo("nl-BE"));

                var message = split[1].Trim();

                reports.Add(new ParkingReport
                {
                    DisplayDate = date,
                    Message = message,
                    Date = dateObject
                });
            }

            return reports;
        }
    }
}
