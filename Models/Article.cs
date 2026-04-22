namespace BusinessNewsApp.Models
{
    // Represents a single news article returned by the NewsAPI
    public class Article
    {
        public Source Source { get; set; } = new();
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
