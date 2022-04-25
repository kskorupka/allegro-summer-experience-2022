using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

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
                    if (repositories != null) owner = await ProcessOwner(response);
                    if (repositories != null && owner != null)
                    {
                        Console.WriteLine("List of " + response + "'s repositories:\n");
                        owner.FillLanguages(repositories);
                        WriteRepositories(repositories);
                        WriteOwnerData(owner);
                        Console.WriteLine("Press any button to continue");
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
                foreach(Repository repository in repositories)
                    repository.Languages = await ProcessLanguages(repository.Language_URL);
                return repositories;
            }
            catch (HttpRequestException ex)
            {
                Console.Clear();
                Console.WriteLine("Connection error");
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any button to continue");
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
            }
            catch (HttpRequestException ex)
            {
                Console.Clear();
                Console.WriteLine("Connection error");
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any button to continue");
                return null;
            }
        }
        private static async Task<Dictionary<String,int>> ProcessLanguages(String URL)
        {
            try
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
                client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
                var streamTask = client.GetStreamAsync(URL);
                var languages = await JsonSerializer.DeserializeAsync<Dictionary<String,int>>(await streamTask);
                return languages;
            }
            catch(HttpRequestException ex)
            {
                Console.Clear();
                Console.WriteLine("Connection error");
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any button to continue");
                return null;
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
                Console.WriteLine("languages: \n");
                foreach(String language in repo.Languages.Keys)
                {
                    Console.WriteLine("language: " + language);
                    Console.WriteLine("bytes of code: " + repo.Languages[language] + "\n");
                }
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
                Console.WriteLine("bytes of code: " + owner.Languages[language] + "\n");
            }
        }
    }
}
