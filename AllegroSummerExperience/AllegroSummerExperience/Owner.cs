using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace AllegroSummerExperience
{
    /// <summary>
    /// Class used to represent repositories' owner (user)
    /// </summary>
    public class Owner
    {
        [JsonPropertyName("login")]
        public string Login { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("bio")]
        public string Bio { get; set; }
        public Dictionary<String, int> Languages = new Dictionary<string, int>();
        [JsonPropertyName("public_repos")]
        public int CountOfRepositories { get; set; }
        /// <summary>
        /// Function that fill Languages dictionary. From each repository it gathers information about used languages and bytes of code in each language
        /// </summary>
        /// <param name="repos">List of all repositories</param>
        public void FillLanguages(List<Repository> repos)
        {
            foreach (Repository repo in repos)
            {
                foreach (String language in repo.Languages.Keys)
                {
                    if (language != null && !Languages.Keys.Contains(language)) Languages.Add(language, repo.Languages[language]);
                    else if (language != null) Languages[language] += repo.Languages[language];
                }
            }
        }
    }
}
