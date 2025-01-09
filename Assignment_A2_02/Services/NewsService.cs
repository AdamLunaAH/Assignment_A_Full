using System.Net;
using Newtonsoft.Json;

using Assignment_A2_02.Models;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http.Features;

namespace Assignment_A2_02.Services;

public class NewsService 
{
    readonly string _subscriptionKey = "256970bad92b4d5398613d17fcba4a7f";
    readonly string _endpoint = "https://api.bing.microsoft.com/v7.0/news";
    readonly HttpClient _httpClient = new HttpClient();

    //readonly ConcurrentDictionary<NewsCategory, (NewsResponse newsResponse, DateTime timestamp)> _cachedNews = new ConcurrentDictionary<NewsCategory, (NewsResponse, DateTime)>();

    readonly ConcurrentDictionary<NewsCategory, (NewsResponse newsResponse, DateTime timestamp)> _cachedNews =
        new ConcurrentDictionary<NewsCategory, (NewsResponse, DateTime)>();


    public NewsService()
    {
        _httpClient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
        _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
    }

    public event EventHandler<string> NewsAvailable;

    protected virtual void OnNewsAvailable(string message)
    {
        NewsAvailable?.Invoke(this, message);
    }

    public async Task<NewsResponse> GetNewsAsync(NewsCategory category)
    {


        //if (_cachedNews.TryGetValue((category), out var cachedData))
        //{
        //    if ((DateTime.Now - cachedData.timestamp).TotalMinutes < 1)
        //    {
        //        OnNewsAvailable($"News for {category} is available from cache.");
        //        return cachedData.newsResponse;
        //    }

        //    else 
        //    {
        //        _cachedNews.TryRemove(category, out _);
        //    }
        //}


        // Check if the news for the category is already cached
        if (_cachedNews.TryGetValue(category, out var cachedData))
        {
            // If the cached data is less than 1 minute old, return it
            if ((DateTime.Now - cachedData.timestamp).TotalMinutes < 1)
            {
                OnNewsAvailable($"News for {category} is available from cache.");
                return cachedData.newsResponse;
            }

            // If the cached data is expired, remove it
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

        //NewsResponse newsResponse = await ReadWebApiAsync(category, uri); 

        _cachedNews[category] = (newsResponse, DateTime.Now);

        OnNewsAvailable($"News for {category} is available.");



        return newsResponse;
    }

    // Method to clear the cache (optional, for testing purposes)
    public void ClearCache()
    {
        _cachedNews.Clear();
        Console.WriteLine("Cache cleared.");
    }
    //private async Task<NewsResponse> ReadWebApiAsync(NewsCategory c, string uri)
    //{
    //    HttpResponseMessage response = await _httpClient.GetAsync(uri);
    //    response.EnsureSuccessStatusCode();
    //    //Convert Json to NewsResponse
    //    string content = await response.Content.ReadAsStringAsync();
    //    var newsResponseData = JsonConvert.DeserializeObject<NewsResponse>(content);


    //    var newsResponse = new NewsResponse(

    //        Articles = newsResponseData.Articles.Select(article => new NewsArticle
    //        {
    //            Title = article.Title,
    //            Url = article.Url,
    //            Description = article.Description,
    //            DatePublished = article.DatePublished,
    //            Providers = article.Providers.Select(provider => new NewsProvider
    //            {
    //                Name = provider.Name
    //            }).ToList(),
    //            Image = new NewsImage
    //            {
    //                Thumbnail = new NewsThumbnail
    //                {
    //                    ContentUrl = article.Image.Thumbnail.ContentUrl,
    //                    Width = article.Image.Thumbnail.Width,
    //                    Height = article.Image.Thumbnail.Height
    //                }
    //            }
    //        }).ToList(),



    //        );


    //    return newsResponse;
    //}
}
