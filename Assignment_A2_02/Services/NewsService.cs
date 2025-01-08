using System.Net;
using Newtonsoft.Json;

using Assignment_A2_02.Models;
using System.Collections.Concurrent;

namespace Assignment_A2_02.Services;

public class NewsService 
{
    readonly string _subscriptionKey = "1d6f7a41ba624b65a749f1cd1420391e";
    readonly string _endpoint = "https://api.bing.microsoft.com/v7.0/news";
    readonly HttpClient _httpClient = new HttpClient();

    public NewsService()
    {
        _httpClient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
        _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
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

        return newsResponse;
    }
}
