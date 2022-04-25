using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace AllegroSummerExperience
{
    public class Repository
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("language")]
        public string Language { get; set; }
        [JsonPropertyName("size")]
        public int Size { get; set; }
    }
}
