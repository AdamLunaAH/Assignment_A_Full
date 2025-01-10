using System.Net;
using Newtonsoft.Json;

using A2_News_Assignment.Models;
using System.Collections.Concurrent;

namespace A2_News_Assignment.Services;
public class NewsService
{
    readonly string _subscriptionKey = "256970bad92b4d5398613d17fcba4a7f";
    readonly string _endpoint = "https://api.bing.microsoft.com/v7.0/news";
    readonly HttpClient _httpClient = new HttpClient();

    // ConcurrentDictionaries to store the cached news
    readonly ConcurrentDictionary<NewsCategory, (NewsResponse newsResponse, DateTime timestamp)> _cachedNews =
        new ConcurrentDictionary<NewsCategory, (NewsResponse, DateTime)>();


    public NewsService()
    {
        _httpClient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
        _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);

        // Load the cache from the XML file
        LoadCacheFromXML();
    }

    // Event for when the news is available
    public event EventHandler<string> NewsAvailable;
    protected virtual void OnNewsAvailable(string message)
    {
        NewsAvailable?.Invoke(this, message);
    }

    // Get the news for a category
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
            // If the cache is expired, clear it
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


        // Cache the new data with the current timestamp
        _cachedNews[category] = (newsResponse, DateTime.Now);

        // Save the cache to an XML file
        SaveCacheToXML(category, newsResponse);


        // Message that the news is not from cache (from the Bing server).
        OnNewsAvailable($"News for {category} is available from server. ");

        return newsResponse;
    }

    // Save the cache to an XML file
    private void SaveCacheToXML(NewsCategory category, NewsResponse newsResponse)
    {
        try
        {
            var key = new NewsCacheKey(category, DateTime.Now);
            NewsResponse.Serialize(newsResponse, key.FileName);
            Console.WriteLine($"Cache for {category} saved to: {key.FileName}");
        }
        // Catch any exceptions that occur during the cache save
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save cache for {category}: {ex.Message}");
        }
    }

    // Load the cache from the XML file
    private void LoadCacheFromXML()
    {

        // Get the cache directory from NewsCacheKey.cs
        var cacheDirectory = NewsCacheKey.GetCacheDirectory();

        // Check if the cache directory exists
        if (cacheDirectory == null || !Directory.Exists(cacheDirectory))
        {
            Console.WriteLine("Cache directory does not exist.");
            return;
        }

        // Load the cache from each cache file
        foreach (var file in Directory.GetFiles(cacheDirectory, "Cache-*.xml"))
        {
            try
            {
                // Get the file information
                var fileInfo = new FileInfo(file);

                // Attempt to parse the category from the filename
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                var parts = fileNameWithoutExtension.Split('-');

                // Check if the filename is in the correct format
                if (parts.Length < 2)
                {
                    Console.WriteLine($"Invalid cache file name format: {file}");
                    continue;
                }
                // Check if the category is valid
                if (!Enum.TryParse(parts[1], out NewsCategory category))
                {
                    Console.WriteLine($"Invalid category in cache file name: {file}");
                    continue;
                }

                // Check if the cache is too old (e.g., older than 1 minute)
                if ((DateTime.Now - fileInfo.LastWriteTime).TotalMinutes > 1)
                // Changed to seconds (for testing)
                //if ((DateTime.Now - fileInfo.LastWriteTime).TotalSeconds > 1)
                {
                    // Shows the cache file is too old and will be deleted
                    Console.WriteLine($"Cache file for {category} is too old and will be deleted: {file}");
                    File.Delete(file);
                    continue;
                }

                // Load the cache if it is valid
                var newsResponse = NewsResponse.Deserialize(file);
                _cachedNews[category] = (newsResponse, fileInfo.LastWriteTime);
                Console.WriteLine($"Loaded cache for {category} from: {file}");
            }
            // Catch any exceptions that occur during the cache load
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load cache file: {file}, Error: {ex.Message}");
            }
        }
    }


    // Clear Cache (used for testing)
    public void ClearCache()
    {
        _cachedNews.Clear();
        ClearCacheXML();
        Console.WriteLine("Cache cleared.");
    }

    // Clear the cache from the XML file (used for testing)
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



