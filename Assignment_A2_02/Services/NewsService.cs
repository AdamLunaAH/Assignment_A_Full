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

        OnNewsAvailable($"News for {category} is available.");

        return newsResponse;
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
