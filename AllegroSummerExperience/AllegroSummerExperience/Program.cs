using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AllegroSummerExperience
{
    internal class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static async Task Main()
        {
            WriteWelcomeMessage();
            String response;
            while ((response = Console.ReadLine()) != "Q")
            {
                if (response != "")
                {
                    Console.Clear();
                    List<Repository> repositories;
                    Owner owner = null;
                    repositories = await ProcessRepos(response);
                    if (repositories != null ) owner = await ProcessOwner(response);
                    if(repositories != null && owner != null)
                    {
                        Console.WriteLine("List of " + response + "'s repositories:\n");
                        owner.FillLanguages(repositories);
                        WriteRepositories(repositories);
                        WriteOwnerData(owner);
                    }                     
                }
                Console.ReadKey();
                Console.Clear();
                WriteWelcomeMessage();
            }
        }
        private static async Task<List<Repository>> ProcessRepos(String username)
        {
            try
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
                client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
                var streamTask = client.GetStreamAsync("https://api.github.com/users/" + username + "/repos");
                var repositories = await JsonSerializer.DeserializeAsync<List<Repository>>(await streamTask);
                return repositories;
            } catch (HttpRequestException ex)
            {
                Console.Clear();
                Console.WriteLine("Connection error");
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        private static async Task<Owner> ProcessOwner(String username)
        {
            try
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
                client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
                var streamTask = client.GetStreamAsync("https://api.github.com/users/" + username);
                var owner = await JsonSerializer.DeserializeAsync<Owner>(await streamTask);
                return owner;
            }catch (HttpRequestException ex)
            {
                Console.Clear();
                Console.WriteLine("Connection error");
                Console.WriteLine(ex.Message);
                return null;
            }
            
        }
        public class Repository
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
            [JsonPropertyName("language")]
            public string Language { get; set; }
            [JsonPropertyName("size")]
            public int Size { get; set; }

        }
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
                foreach(Repository repo in repos)
                {
                    if (repo.Language!= null && !Languages.ContainsKey(repo.Language)) Languages.Add(repo.Language, repo.Size);
                    else if(repo.Language != null) Languages[repo.Language] += repo.Size;
                }
            }
        }
        private static void WriteWelcomeMessage()
        {
            Console.WriteLine("Write username to get the repositories and press Enter");
            Console.WriteLine("If you want to leave the app, write 'Q' and press Enter");
        }
        private static void WriteRepositories(List<Repository> repositories)
        {
            foreach (var repo in repositories)
            {
                Console.WriteLine("name: " + repo.Name);
                Console.WriteLine("language: " + repo.Language);
                Console.WriteLine("bites of code: " + repo.Size.ToString());
                Console.WriteLine();
            }
        }
        private static void WriteOwnerData(Owner owner)
        {
            Console.WriteLine("User's data:");
            Console.WriteLine("login: " + owner.Login);
            Console.WriteLine("full name: " + owner.Name);
            Console.WriteLine("bio: " + owner.Bio);
            Console.WriteLine("used languages: \n");
            foreach (String language in owner.Languages.Keys)
            {
                Console.WriteLine("language: " + language);
                Console.WriteLine("bites of code: " + owner.Languages[language] + "\n");
            }
        }
    }
}
