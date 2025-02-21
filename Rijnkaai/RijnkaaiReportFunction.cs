using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json;
using Rijnkaai.Abstractions;
using Rijnkaai.Domain;

namespace Rijnkaai
{
    public class RijnkaaiReportFunction
    {
        private readonly IEnumerable<INotificationService> _notificationServices;
        private readonly IRijnkaaiService _rijnkaaiService;
        private readonly string ContainerName = "rijnkaaijsonlogs";
        private readonly string BlobName = "parking_report.json";
        private readonly BlobClient _blobClient;

        public RijnkaaiReportFunction(IEnumerable<INotificationService> registeredServices, IRijnkaaiService rijnkaaiService)
        {
            _notificationServices = registeredServices;
            _rijnkaaiService = rijnkaaiService;

            var blobClient = new BlobServiceClient(Environment.GetEnvironmentVariable("BlobStorage-ConnectionString"));

            _blobClient = blobClient.GetBlobContainerClient(ContainerName).GetBlobClient(BlobName);
        }

        [Function("GetClosingTimes")]
        public async Task Run([TimerTrigger("0 0 16 * * *", RunOnStartup = true)] TimerInfo timerInfo)
        {
            var toPostObject = await _rijnkaaiService.GetRijnkaaiClosedDates();

            var currentContent = await _blobClient.DownloadContentAsync();

            var reports = JsonConvert.DeserializeObject<IEnumerable<ParkingReport>>(currentContent.Value.Content.ToString());

            var sameBatch = reports?.Sum(x => x.StartDate.Ticks) == toPostObject.Sum(x => x.StartDate.Ticks);

            if (!sameBatch)
            {
                await Task.WhenAll(_notificationServices.Select(x => x.PostMultipleReports(toPostObject)));

                await _blobClient.UploadAsync(BinaryData.FromObjectAsJson(toPostObject), overwrite: true);
            }
        }

        [Function("DailyNotification")]
        public async Task SendDailyNotification([TimerTrigger("0 0 17 * * *")] TimerInfo timerInfo)
        {

            var content = await _blobClient.DownloadContentAsync();

            var reports = JsonConvert.DeserializeObject<IEnumerable<ParkingReport>>(content.Value.Content.ToString());

            if (reports is null || reports.Count() == 0)
            {
                return;
            }

            var nextDayReport = reports.Where(x => x.StartDate <= DateTime.Now.Date.AddDays(1) && DateTime.Now.Date.AddDays(1) <= x.EndDate).FirstOrDefault();

            if (nextDayReport is not null)
            {
                await Task.WhenAll(_notificationServices.Select(x => x.PostSingleReport(nextDayReport)));
            }
        }
    }
}
