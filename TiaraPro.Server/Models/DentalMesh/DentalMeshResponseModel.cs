using Newtonsoft.Json;

namespace TiaraPro.Server.Models.DentalMesh
{
    public class DentalMeshResponseModel
    {
        [JsonProperty("guid")]
        public string Guid { get; set; }
    }
} 