using Newtonsoft.Json;

namespace Rijnkaai.Domain
{
    public class SlackText
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "mrkdwn";

        [JsonProperty("text")]
        public string Text { get; set; } = string.Empty;

        public static SlackText CreateDefaultTextBlock(string text)
        {
            return new() { Text = text };
        }

        public static SlackText CreatePlainTextBlock(string text)
        {
            return new() { Type = "plain_text", Text = text };
        }
    }
}
