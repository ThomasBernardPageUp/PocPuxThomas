using System;
using Newtonsoft.Json;

namespace PocPuxThomas.Models.DTO.Down
{
    public class LocationDownDTO
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
