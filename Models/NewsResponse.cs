namespace BusinessNewsApp.Models
{
    // Wraps the full JSON response returned by the NewsAPI
    public class NewsResponse
    {
        public string Status { get; set; } = string.Empty;
        public List<Article> Articles { get; set; } = new();
    }
}
