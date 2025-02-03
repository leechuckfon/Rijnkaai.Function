using Newtonsoft.Json;

namespace Rijnkaai.Domain
{
    public class SlackMessage
    {
        [JsonProperty("color")]
        public string Color { get; set; } = string.Empty;
        [JsonProperty("blocks")]
        public IEnumerable<SlackBlock> Blocks { get; set; } = Enumerable.Empty<SlackBlock>();
    }
}
