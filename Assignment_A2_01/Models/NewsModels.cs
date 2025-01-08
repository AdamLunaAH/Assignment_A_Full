using Newtonsoft.Json;

namespace Assignment_A2_01.Models;

public enum NewsCategory { Business, Entertainment, Technology, Sports, World };

public class NewsResponse
{
    public NewsCategory Category {get; set;}

    [JsonProperty("value")]
    public List<NewsArticle> Articles { get; set; }
}

public class NewsArticle
{
    [JsonProperty("name")]
    public string Title { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("datePublished")]
    public DateTime DatePublished {get; set; }

    [JsonProperty("provider")]
    public List<NewsProvider> Providers { get; set; }

    [JsonProperty("image")]
    public NewsImage Image { get; set; }
}

public class NewsProvider
{
    [JsonProperty("name")]
    public string Name { get; set; }
}

public class NewsImage
{
    [JsonProperty("thumbnail")]
    public NewsThumbnail Thumbnail { get; set; }
}

public class NewsThumbnail
{
    [JsonProperty("contentUrl")]
    public string ContentUrl { get; set; }

    [JsonProperty("width")]
    public int Width { get; set; }

    [JsonProperty("height")]
    public int Height { get; set; }
}
