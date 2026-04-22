using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using BusinessNewsApp.Models;

namespace BusinessNewsApp.Controllers;

public class HomeController : Controller
{
    // IHttpClientFactory is injected to create HTTP clients safely
    private readonly IHttpClientFactory _httpClientFactory;
    // IConfiguration lets us read values from appsettings.json
    private readonly IConfiguration _configuration;

    public HomeController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    // Async Index action — fetches business headlines from NewsAPI
    public async Task<IActionResult> Index()
    {
        // Read the API key from appsettings.json → "NewsApi" → "ApiKey"
        var apiKey = _configuration["NewsApi:ApiKey"];

        var client = _httpClientFactory.CreateClient();

        // Build the NewsAPI endpoint URL for top US business headlines
        var url = $"https://newsapi.org/v2/top-headlines?country=us&category=business&apiKey={apiKey}";

        // Send the GET request and read the response body as a string
        var responseString = await client.GetStringAsync(url);

        // Deserialize JSON into NewsResponse — case-insensitive to match JSON property names
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var newsResponse = JsonSerializer.Deserialize<NewsResponse>(responseString, options);

        // Pass the list of articles to the view (empty list as fallback)
        var articles = newsResponse?.Articles ?? new List<Article>();
        return View(articles);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
