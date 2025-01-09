using System.Net;
using Newtonsoft.Json;

using Assignment_A2_04.Models;
using System.Collections.Concurrent;

namespace Assignment_A2_04.Services;
public class NewsService
{
    readonly string _subscriptionKey = "256970bad92b4d5398613d17fcba4a7f";
    readonly string _endpoint = "https://api.bing.microsoft.com/v7.0/news";
    readonly HttpClient _httpClient = new HttpClient();

    readonly ConcurrentDictionary<NewsCategory, (NewsResponse newsResponse, DateTime timestamp)> _cachedNews =
        new ConcurrentDictionary<NewsCategory, (NewsResponse, DateTime)>();


    public NewsService()
    {
        _httpClient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
        _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);

        // Load the cache from the XML file
        LoadCacheFromXML();
    }

    //Event declaration
    public event EventHandler<string> NewsAvailable;
    protected virtual void OnNewsAvailable(string message)
    {
        NewsAvailable?.Invoke(this, message);
    }

    public async Task<NewsResponse> GetNewsAsync(NewsCategory category)
    {


        // Check if the news for the category is in the cache


        if (_cachedNews.TryGetValue(category, out var cachedData))
        {
            // Check if the cache is still valid (less than 1 minute old)
            if ((DateTime.Now - cachedData.timestamp).TotalMinutes < 1)
            {
                // Message that the news are from cache
                OnNewsAvailable($"News for {category} is available from cache.");
                return cachedData.newsResponse;
            }

            // If the cache is expired, clear it and fetch new data
            _cachedNews.TryRemove(category, out _);
        }

        //To ensure not too many requests per second for BingNewsApi free plan
        await Task.Delay(2000);

        // make the http request and ensure success
        string uri = $"{_endpoint}?mkt=en-us&category={Uri.EscapeDataString(category.ToString())}";
        HttpResponseMessage response = await _httpClient.GetAsync(uri);
        response.EnsureSuccessStatusCode();


        //Convert Json to NewsResponse
        string content = await response.Content.ReadAsStringAsync();
        var newsResponse = JsonConvert.DeserializeObject<NewsResponse>(content);
        newsResponse.Category = category;


        //// Cache the new data with the current timestamp
        _cachedNews[category] = (newsResponse, DateTime.Now);
        ///* Used for testing the cache expiration check (Removes 10 minutes from the cache timestamp to always make it to old).
        //This is faster than using a long pause between the Tasks */
        ////_cachedNews[category] = (newsResponse, DateTime.Now.AddMinutes(-10));

        SaveCacheToXML(category, newsResponse);


        // Message that the mews is not from cache (from the Bing server).
        OnNewsAvailable($"News for {category} is available from server. ");

        return newsResponse;
    }

    // Clear Cache (used for testing purposes)
    public void ClearCache()
    {
        _cachedNews.Clear();
        ClearCacheXML();
        Console.WriteLine("Cache cleared.");
    }

    private void SaveCacheToXML(NewsCategory category, NewsResponse newsResponse)
    {
        try
        {
            var key = new NewsCacheKey(category, DateTime.Now);
            NewsResponse.Serialize(newsResponse, key.FileName);
            Console.WriteLine($"Cache for {category} saved to: {key.FileName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save cache for {category}: {ex.Message}");
        }
    }

    private void LoadCacheFromXML()
    {

        foreach (var category in Enum.GetValues(typeof(NewsCategory)).Cast<NewsCategory>())
        {
            try
            {
                var key = new NewsCacheKey(category, DateTime.Now);
                if (key.CacheExist)
                {
                    

                    // Check if the cache is too old (e.g., older than 1 minute)
                    

                    // Load the cache if it is valid
                    var newsResponse = NewsResponse.Deserialize(key.FileName);
                    _cachedNews[category] = (newsResponse, DateTime.Now);
                    Console.WriteLine($"Loaded cache for {category} from: {key.FileName}");
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load cache for {category}: {ex.Message}");
            }
        }

    }

    private void ClearCacheXML()
    {
        foreach (var category in Enum.GetValues(typeof(NewsCategory)).Cast<NewsCategory>())
        {
            try
            {
                var key = new NewsCacheKey(category, DateTime.Now);
                if (key.CacheExist)
                {
                    File.Delete(key.FileName);
                    Console.WriteLine($"Deleted cache for {category} from XML-file at: {key.FileName}.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete cache for {category}: {ex.Message}");
            }

        }
    }
}



