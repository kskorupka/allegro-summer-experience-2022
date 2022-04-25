using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace AllegroSummerExperience
{
    public class Repository
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        public Dictionary<String, int> Languages = new Dictionary<string, int>();
        [JsonPropertyName("languages_url")]
        public String Language_URL { get; set; }
    }
}
