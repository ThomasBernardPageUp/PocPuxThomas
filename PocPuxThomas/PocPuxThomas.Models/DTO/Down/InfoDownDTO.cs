using System;
using Newtonsoft.Json;

namespace PocPuxThomas.Models.DTO.Down
{
    public class InfoDownDTO
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("pages")]
        public int Pages { get; set; }

        [JsonProperty("next")]
        public string Next { get; set; }

        [JsonProperty("prev")]
        public object Prev { get; set; }
    }
}
