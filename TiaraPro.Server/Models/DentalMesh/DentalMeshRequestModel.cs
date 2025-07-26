using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace TiaraPro.Server.Models.DentalMesh
{
    // Define the classes
    public class DentalMeshRequestModel
    {
        [JsonProperty("guid")]
        public string Guid { get; set; } = string.Empty;

        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; } = new Metadata();

        [JsonProperty("url")]
        public string Url { get; set; } = string.Empty;
    }

    public class Metadata
    {
        [JsonProperty("add_metadata")]
        public string Add_Metadata { get; set; } = "False";

        [JsonProperty("create_short_url")]
        public string Create_Short_Url { get; set; } = "True";

        [JsonProperty("no_airway")]
        public string No_Airway { get; set; } = "False";

        [JsonProperty("no_combined_teeth")]
        public string No_Combined_Teeth { get; set; } = "False";

        [JsonProperty("no_cranial_base")]
        public string No_Cranial_Base { get; set; } = "False";

        [JsonProperty("no_mandible_nerve")]
        public string No_Mandible_Nerve { get; set; } = "False";

        [JsonProperty("offset")]
        public int Offset { get; set; } = 22;

        [JsonProperty("stl_airway")]
        public double Stl_Airway { get; set; } = 1;

        [JsonProperty("stl_cranial_base")]
        public double Stl_Cranial_Base { get; set; } = 0.5;

        [JsonProperty("stl_mandible")]
        public double Stl_Mandible { get; set; } = 1;

        [JsonProperty("stl_mandible_nerve")]
        public double Stl_Mandible_Nerve { get; set; } = 1;

        [JsonProperty("stl_maxilla")]
        public double Stl_Maxilla { get; set; } = 1;

        [JsonProperty("stl_upper_skull")]
        public double Stl_Upper_Skull { get; set; } = 0.5;
    }

    public class LowercaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLower(); // Convert property names to lowercase
        }
    }
} 