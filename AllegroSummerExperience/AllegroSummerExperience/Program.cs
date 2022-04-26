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
    /// <summary>
    /// Class <c>Program</c> is used to perform the solution
    /// <remarks>Uses Repository and Owner classes</remarks>
    /// <seealso cref="Owner"/>
    /// <seealso cref="Repository"/>
    /// </summary>
    internal class Program
    {
        private static readonly HttpClient client = new HttpClient();
        /// <summary>
        /// Main function
        /// </summary>
        private static async Task Main()
        {
            //Authorization
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes("fbebb3bf766b8408be30" + ":" + "8eb82bc12ba37a1ac0b55e8d97944f5c3517b8ca")));
            WriteWelcomeMessage();
            String response;
            while ((response = Console.ReadLine()) != "Q")
            {
                if (response != "")
                {
                    Console.Clear();
                    Owner owner = await ProcessOwner(response);
                    List<Repository> all_repositories = new List<Repository>();
                    if(owner != null)
                    {
                        for (int i = 1; i <= GetCountOfPages(owner.CountOfRepositories); i++)
                        {
                            List<Repository> repository = await ProcessRepos(response, i);
                            if (repository != null) all_repositories.AddRange(repository);
                        }
                    }
                    if (all_repositories != null && owner != null)
                    {
                        Console.WriteLine("List of " + response + "'s repositories:\n");
                        owner.FillLanguages(all_repositories);
                        WriteRepositories(all_repositories);
                        WriteOwnerData(owner);
                        Console.WriteLine("Press any button to continue");
                    }  
                }
                Console.ReadKey();
                Console.Clear();
                WriteWelcomeMessage();
            }
        }
        /// <summary>
        /// This function is used to process all repositories on given page
        /// </summary>
        /// <param name="username">user's name</param>
        /// <param name="page">given page</param>
        /// <returns>list of all repositories from given page</returns>
        /// <exception cref="HttpRequestException"></exception>
        private static async Task<List<Repository>> ProcessRepos(String username, int page)
        {
            try
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
                client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
                var streamTask = client.GetStreamAsync("https://api.github.com/users/" + username + "/repos?page=" + page);
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
        /// <summary>
        /// This function is used to gather all important data about the user
        /// </summary>
        /// <param name="username">user's name</param>
        /// <returns>User(Repositories' owner)</returns>
        /// <exception cref="HttpRequestException"></exception>
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
        /// <summary>
        /// This function is used to get all languages of one repository
        /// </summary>
        /// <param name="URL">"languages_url" in repository</param>
        /// <returns>Dictionary<String,int> where language is the key and bytes of code is the value</String></returns>
        /// <exception cref="HttpRequestException"></exception>
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
        /// <summary>
        /// Function that prints welcome messages
        /// </summary>
        private static void WriteWelcomeMessage()
        {
            Console.WriteLine("Write username to get the repositories and press Enter");
            Console.WriteLine("If you want to leave the app, write 'Q' and press Enter");
        }
        /// <summary>
        /// Function that prints all repositories with used languages and bytes of code in each language
        /// </summary>
        /// <param name="repositories">List of repositories</param>
        private static void WriteRepositories(List<Repository> repositories)
        {
            foreach (var repo in repositories)
            {
                Console.WriteLine("repository name: " + repo.Name);
                Console.WriteLine("used languages: \n");
                if (repo.Languages.Keys.Count == 0) Console.WriteLine("None");
                else
                {
                    foreach (String language in repo.Languages.Keys)
                    {
                        Console.WriteLine("language: " + language);
                        Console.WriteLine("bytes of code: " + repo.Languages[language] + "\n");
                    }
                }
                Console.WriteLine("========================================");
                Console.WriteLine();
            }
        }
        /// <summary>
        /// Function that prints all user's important data with used languages and bytes of code in each language
        /// </summary>
        /// <param name="owner"></param>
        private static void WriteOwnerData(Owner owner)
        {
            Console.WriteLine("User's data:");
            Console.WriteLine("login: " + owner.Login);
            Console.WriteLine("full name: " + owner.Name);
            Console.WriteLine("bio: " + owner.Bio);
            Console.WriteLine("used languages: \n");
            if(owner.Languages.Keys.Count == 0) Console.WriteLine("None");
            else
            {
                foreach (String language in owner.Languages.Keys)
                {
                    Console.WriteLine("language: " + language);
                    Console.WriteLine("bytes of code: " + owner.Languages[language] + "\n");
                }
            }
        }
        /// <summary>
        /// Function that counts how many repository pages user has
        /// </summary>
        /// <param name="CountOfRepositories">Count of user's repositories</param>
        /// <returns>Count of pages</returns>
        private static int GetCountOfPages(int CountOfRepositories)
        {
            if (CountOfRepositories % 30 == 0) return CountOfRepositories / 30;
            else return CountOfRepositories / 30 + 1;
        }
    }
}
