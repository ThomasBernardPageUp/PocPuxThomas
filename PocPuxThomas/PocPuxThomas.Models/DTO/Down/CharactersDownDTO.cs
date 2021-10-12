using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PocPuxThomas.Models.DTO.Down
{
    public class CharactersDownDTO
    {
        [JsonProperty("info")]
        public InfoDownDTO Info { get; set; }

        [JsonProperty("results")]
        public List<CharacterDownDTO> Results { get; set; }
    }
}
