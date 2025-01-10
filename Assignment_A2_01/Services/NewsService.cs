using System.Net;
using Newtonsoft.Json;

using Assignment_A2_01.Models;

namespace Assignment_A2_01.Services;
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

    public async Task<NewsResponse> GetNewsAsync(NewsCategory category)
    {
        // make the http request and ensure success
        string uri = $"{_endpoint}?mkt=en-us&category={Uri.EscapeDataString(category.ToString())}";
        HttpResponseMessage response = await _httpClient.GetAsync(uri);
        response.EnsureSuccessStatusCode();

        //To ensure not too many requests per second for BingNewsApi free plan
        await Task.Delay(2000);

        //Convert Json to NewsResponse
        string content = await response.Content.ReadAsStringAsync();
        var newsResponse = JsonConvert.DeserializeObject<NewsResponse>(content);
        newsResponse.Category = category;


        //var forecast = new Forecast
        //{
        //    City = wd.city.name,
        //    Items = wd.list.Select(item => new ForecastItem
        //    {
        //        DateTime = UnixTimeStampToDateTime(item.dt),
        //        Temperature = item.main.temp,
        //        WindSpeed = item.wind.speed,
        //        Description = item.weather.FirstOrDefault().description,
        //        Icon = $"http://openweathermap.org/img/w/{item.weather.First().icon}.png"
        //    }).ToList()
        //};


        return newsResponse;
    }
}

