using RestSharp;
using System.Net.Http.Headers;
using System.Text.Json;

namespace TMDB_CLI_Tool
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            while (true)
            {
                PromptMovieType();
                string? input = Console.ReadLine();

                if (int.TryParse(input, out int option) && option >= 1 && option <= 5)
                {
                    if (option == 5)
                    {
                        Console.WriteLine("Exiting the application.");
                        break;
                    }
                    string movieTypeUrl = GetMovieType(option);
                    await FetchMovies(movieTypeUrl);

                    Console.WriteLine();
                    Console.WriteLine(new string('=', 50));
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number between 1 and 5.");
                }
            }
        }

        public static void PromptMovieType()
        {
            Console.WriteLine("Options Types:");
            Console.WriteLine("1. Playing.");
            Console.WriteLine("2. Popular.");
            Console.WriteLine("3. Top.");
            Console.WriteLine("4. Upcoming.");
            Console.WriteLine("5. Exit.");
            Console.Write("Please select an option (1-5): ");
        }

        public static string GetMovieType(int option)
        {
            switch (option)
            {

                case 1:
                    return "https://api.themoviedb.org/3/movie/now_playing";
                case 2:
                    return "https://api.themoviedb.org/3/discover/movie?include_adult=false&include_video=false&language=en-US&page=1&sort_by=popularity.desc";
                case 3:
                    return "https://api.themoviedb.org/3/discover/movie?include_adult=false&include_video=false&language=en-US&page=1&sort_by=vote_average.desc&without_genres=99,10755&vote_count.gte=200";
                case 4:
                    return "https://api.themoviedb.org/3/discover/movie?include_adult=false&include_video=false&language=en-US&page=1&sort_by=popularity.desc&with_release_type=2";
                default:
                    throw new ArgumentException("Invalid option selected.");
            }
        }

        public static async Task FetchMovies(string url)
        {
            try
            {
                // Use the actual API URL passed from GetMovieType
                var options = new RestClientOptions(url);
                var client = new RestClient(options);
                var request = new RestRequest();

                // Replace this with your real Bearer token from TMDB
                string bearerToken = "eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiIyNzQ3MjBlZjY2NTI3YmY3NjQwMzU1ZGI2NTA2ODc0YSIsIm5iZiI6MTc0OTc2ODcxMS44MjYsInN1YiI6IjY4NGI1YTA3MTkyNDRlNzkxODNkZWVjZCIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.KVK8rJJ_JFI1xHZoG3K1NP0NUXTcqRAbT7t8poDFUtU";

                // Add Authorization header
                request.AddHeader("Authorization", $"Bearer {bearerToken}");
                request.AddHeader("accept", "application/json");

                var response = await client.GetAsync(request);

                if (!response.IsSuccessful)
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ErrorMessage}");
                    return;
                }

                var serializedResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(response.Content);
                if (serializedResponse == null || !serializedResponse.ContainsKey("results"))
                {
                    Console.WriteLine("No results found.");
                    return;
                }

                var results = serializedResponse["results"] as JsonElement?;

                if (results.HasValue && results.Value.ValueKind == JsonValueKind.Array)
                {
                    foreach (var movie in results.Value.EnumerateArray())
                    {
                        string title = movie.GetProperty("title").GetString() ?? "Unknown Title";
                        string releaseDate = movie.GetProperty("release_date").GetString() ?? "Unknown Release Date";
                        Console.WriteLine($"Title: {title}, Release Date: {releaseDate}");
                    }
                }
                else
                {
                    Console.WriteLine("No movies found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
