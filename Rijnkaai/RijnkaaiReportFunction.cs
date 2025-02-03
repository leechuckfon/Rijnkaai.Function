using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json;
using Rijnkaai.Abstractions;
using Rijnkaai.Domain;

namespace Rijnkaai
{
    public class RijnkaaiReportFunction
    {
        private readonly ISlackService _slackService;
        private readonly IRijnkaaiService _rijnkaaiService;
        private readonly string ContainerName = "rijnkaaijsonlogs";
        private readonly string BlobName = "parking_report.json";
        private readonly BlobClient _blobClient;

        public RijnkaaiReportFunction(ISlackService service, IRijnkaaiService rijnkaaiService)
        {
            _slackService = service;
            _rijnkaaiService = rijnkaaiService;

            var blobClient = new BlobServiceClient(Environment.GetEnvironmentVariable("BlobStorage-ConnectionString"));

            _blobClient = blobClient.GetBlobContainerClient(ContainerName).GetBlobClient(BlobName);
        }

        [Function("GetClosingTimes")]
        public async Task Run([TimerTrigger("0 0 16 * * *")] TimerInfo timerInfo)
        {
            var toPostObject = await _rijnkaaiService.GetRijnkaaiClosedDates();

            var currentContent = await _blobClient.DownloadContentAsync();

            var reports = JsonConvert.DeserializeObject<IEnumerable<ParkingReport>>(currentContent.Value.Content.ToString());

            var sameBatch = reports?.Sum(x => x.Date.Ticks) == toPostObject.Sum(x => x.Date.Ticks);

            if (!sameBatch)
            {
                await _slackService.PostMessageToSlack(toPostObject);

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

            var nextDayReport = reports.Where(x => x.Date == DateTime.Now.Date.AddDays(1)).FirstOrDefault();

            if (nextDayReport is not null)
            {
                await _slackService.PostSingleMessageToSlack(nextDayReport);
            }
        }
    }
}
