using Newtonsoft.Json;

namespace Rijnkaai.Domain
{
    public class SlackBlock
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "section";

        [JsonProperty("text")]
        public SlackText? Text { get; set; } = null;


        public bool ShouldSerializeText() => Text is not null;

        public static SlackBlock CreateDivider()
        {
            return new() { Type = "divider" };
        }
    }
}
