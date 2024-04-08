using System.Text.Json.Serialization;

namespace AgentMonitoringService
{
    public class EventInput
    {
        [JsonPropertyName("agent_id")]
        public string AgentId { get; set; }
        [JsonPropertyName("image_id")]
        public string ImageId { get; set; }
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp {  get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
