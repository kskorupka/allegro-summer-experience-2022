using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace AllegroSummerExperience
{
    public class Owner
    {
        [JsonPropertyName("login")]
        public string Login { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("bio")]
        public string Bio { get; set; }
        public Dictionary<String, int> Languages = new Dictionary<string, int>();
        public void FillLanguages(List<Repository> repos)
        {
            foreach (Repository repo in repos)
            {
                if (repo.Language != null && !Languages.ContainsKey(repo.Language)) Languages.Add(repo.Language, repo.Size);
                else if (repo.Language != null) Languages[repo.Language] += repo.Size;
            }
        }
    }
}
