using Rijnkaai.Abstractions;
using Rijnkaai.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rijnkaai.Business
{
    public class TeamsService : INotificationService
    {
        private readonly HttpClient _client;

        public TeamsService(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
        }
        public async Task PostMultipleReports(IEnumerable<ParkingReport> report)
        {
            await _client.PostAsync(Environment.GetEnvironmentVariable("Teams-PostUrl"), new StringContent(JsonSerializer.Serialize(report), Encoding.UTF8, "application/json"));
        }

        public async Task PostSingleReport(ParkingReport parkingReport)
        {
            await _client.PostAsync(Environment.GetEnvironmentVariable("Teams-PostUrl"), new StringContent(JsonSerializer.Serialize(parkingReport), Encoding.UTF8, "application/json"));
        }
    }
}
