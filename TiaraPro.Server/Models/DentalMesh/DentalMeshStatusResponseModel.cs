using Newtonsoft.Json;

namespace TiaraPro.Server.Models.DentalMesh
{
    public class DentalMeshStatusResponseModel
    {
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("inference_progress")]
        public string InferenceProgress { get; set; }
        [JsonProperty("signed_download_url")]
        public string SignedDownloadUrl { get; set; }
        [JsonProperty("enqueued_at")]
        public string EnqueuedAt { get; set; }
        [JsonProperty("processed_at")]
        public string ProcessedAt { get; set; }
        [JsonProperty("completed_at")]
        public string CompletedAt { get; set; }
        [JsonProperty("short_download_url")]
        public string ShortDownloadUrl { get; set; }
        [JsonProperty("short_viewer_url")]
        public string ShortViewerUrl { get; set; }
    }
} 