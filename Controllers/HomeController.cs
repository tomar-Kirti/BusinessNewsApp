using System.Diagnostics; // Used for tracking request/activity info (used in Error method)
using System.Text.Json; // Used to deserialize JSON response from API
using Microsoft.AspNetCore.Mvc; // Core MVC framework
using BusinessNewsApp.Models; // Access to models like Article and NewsResponse

namespace BusinessNewsApp.Controllers;

public class HomeController : Controller
{
    // IHttpClientFactory is injected to create HTTP clients safely (best practice vs new HttpClient())
    private readonly IHttpClientFactory _httpClientFactory;

    // IConfiguration lets us read values from appsettings.json (like API keys)
    private readonly IConfiguration _configuration;

    // Constructor where dependencies are injected automatically by ASP.NET Core
    public HomeController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    // Main action method that loads the homepage
    public async Task<IActionResult> Index()
    {
        // Read API key from configuration file (appsettings.json)
        var apiKey = _configuration["NewsApi:ApiKey"];

        // Create HTTP client instance using factory
        var client = _httpClientFactory.CreateClient();

        // Build API URL with query parameters (US business headlines)
        var url = $"https://newsapi.org/v2/top-headlines?country=us&category=business&apiKey={apiKey}";

        // Send GET request to NewsAPI
        var response = await client.GetAsync(url);

        // Read API response content as string (JSON format)
        var responseString = await response.Content.ReadAsStringAsync();

        // If API call fails (e.g., invalid key, rate limit, etc.)
        if (!response.IsSuccessStatusCode)
        {
            // Pass error message to the view using ViewBag
            ViewBag.ApiError = $"NewsAPI error {(int)response.StatusCode}: {responseString}";

            // Return empty list to avoid crashing the UI
            return View(new List<Article>());
        }

        // Configure JSON deserialization to ignore case differences in property names
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // Convert JSON string into C# object (NewsResponse model)
        var newsResponse = JsonSerializer.Deserialize<NewsResponse>(responseString, options);

        // Extract articles list safely (avoid null issues)
        var articles = newsResponse?.Articles ?? new List<Article>();

        // Pass articles to the view for rendering
        return View(articles);
    }

    // Privacy page action (default template)
    public IActionResult Privacy()
    {
        return View();
    }

    // Error handling method (default template)
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        // Returns error view with request ID for debugging
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
