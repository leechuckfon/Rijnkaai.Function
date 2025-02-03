using Newtonsoft.Json;
using Rijnkaai.Abstractions;
using Rijnkaai.Domain;
using System.Text;


namespace Rijnkaai.Business
{
    public class SlackService : ISlackService
    {
        private HttpClient client;

        public SlackService(IHttpClientFactory factory)
        {
            client = factory.CreateClient();
        }

        public async Task PostMessageToSlack(IEnumerable<ParkingReport> parkingReport)
        {
            var reports = parkingReport.SelectMany(x =>
            {
                return new SlackBlock[]
                {
                    new SlackBlock
                    {
                        Text = SlackText.CreateDefaultTextBlock($"⛔ *{x.DisplayDate}* \n {x.Message}")
                    },
                    SlackBlock.CreateDivider()
                };
            });

            var header = new SlackBlock
            {
                Type = "header",
                Text = SlackText.CreatePlainTextBlock("Rijnkaai gesloten op volgende datums:")
            };

            var content = JsonConvert.SerializeObject(
                new SlackMessage
                {
                    Color = "#ff0000",
                    Blocks = [
                        header,
                        ..reports.SkipLast(1)
                    ]
                });

            _ = await client.PostAsync(Environment.GetEnvironmentVariable("Slack-PostUrl"), new StringContent(content, Encoding.UTF8, "application/json"));
        }

        public async Task PostSingleMessageToSlack(ParkingReport parkingReport)
        {
            var content = JsonConvert.SerializeObject(
                new SlackMessage
                {
                    Color = "#ff0000",
                    Blocks = [
                        new SlackBlock
                        {
                            Type = "header",
                            Text = SlackText.CreatePlainTextBlock("🛑 Opgelet! - Rijnkaai morgen gesloten")
                        },
                        new SlackBlock
                        {
                            Text = SlackText.CreateDefaultTextBlock($"⛔ *Morgen {parkingReport.DisplayDate}* \n {parkingReport.Message} \n @channel")
                        }
                    ]
                });

            _ = await client.PostAsync(Environment.GetEnvironmentVariable("Slack-PostUrl"), new StringContent(content, Encoding.UTF8, "application/json"));
        }
    }
}
