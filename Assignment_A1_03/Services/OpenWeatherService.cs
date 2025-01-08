using System.Collections.Concurrent;
using Newtonsoft.Json;

using Assignment_A1_03.Models;

namespace Assignment_A1_03.Services;

public class OpenWeatherService
{
    readonly HttpClient _httpClient = new HttpClient();
    //readonly ConcurrentDictionary<(double, double, string), Forecast> _cachedGeoForecasts = new ConcurrentDictionary<(double, double, string), Forecast>();
    //readonly ConcurrentDictionary<(string, string), Forecast> _cachedCityForecasts = new ConcurrentDictionary<(string, string), Forecast>();

    readonly ConcurrentDictionary<(double, double, string), (Forecast forecast, DateTime timestamp)> _cachedGeoForecasts = new ConcurrentDictionary<(double, double, string), (Forecast, DateTime)>();
    readonly ConcurrentDictionary<(string, string), (Forecast forecast, DateTime timestamp)> _cachedCityForecasts = new ConcurrentDictionary<(string, string), (Forecast, DateTime)>();


    // Your API Key
    readonly string apiKey = "d11de2c96e160e2d3350ad3db04c75bc";
    //readonly string apiKey = "";


    //Event declaration
    public event EventHandler<string> WeatherForecastAvailable;
    protected virtual void OnWeatherForecastAvailable (string message)
    {
        WeatherForecastAvailable?.Invoke(this, message);
    }

    public async Task<Forecast> GetForecastAsync(string City)
    {
        //part of cache code here to check if forecast in Cache
        //generate an event that shows forecast was from cache
        //Your code

        // Check if the forecast for the city is in the cache
        if (_cachedCityForecasts.TryGetValue((City, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName), out var cachedData))
        {
            // Check if the cache is still valid (less than 1 minute old)
            if ((DateTime.Now - cachedData.timestamp).TotalMinutes < 1)
            {
                // Message that the forecast is from cache
                OnWeatherForecastAvailable($"Weather forecast for {City} is available from cache.");
                return cachedData.forecast;
            }
            else
            {
                // If the cache is expired, clear it and fetch new data
                _cachedCityForecasts.TryRemove((City, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName), out _);
            }
        }

        //https://openweathermap.org/current
        var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        var uri = $"https://api.openweathermap.org/data/2.5/forecast?q={City}&units=metric&lang={language}&appid={apiKey}";

        Forecast forecast = await ReadWebApiAsync(uri);

        //part of event and cache code here
        //generate an event with different message if cached data
        //Your code

        // Cache the new data with the current timestamp
        _cachedCityForecasts[(City, language)] = (forecast, DateTime.Now);

        // Message that the forecast is not from cache (from the OpenWeatherAPI server).
        OnWeatherForecastAvailable($"Weather forecast for {City} is available from server.");

        return forecast;

    }
    public async Task<Forecast> GetForecastAsync(double latitude, double longitude)
    {
        //part of cache code here to check if forecast in Cache
        //generate an event that shows forecast was from cache
        //Your code

        // Check if the forecast for the coordinates is in the cache
        if (_cachedGeoForecasts.TryGetValue((latitude, longitude, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName), out var cachedData))
        {
            // Check if the cache is still valid (less than 1 minute old)
            if ((DateTime.Now - cachedData.timestamp).TotalMinutes < 1)
            {
                // Message that the forecast is from cache
                OnWeatherForecastAvailable($"Weather forecast for coordinates ({latitude}, {longitude}) is available from cache.");
                return cachedData.forecast;
            }
            else
            {
                // If the cache is expired, clear it and fetch new data
                _cachedGeoForecasts.TryRemove((latitude, longitude, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName), out _);
            }
        }


        //https://openweathermap.org/current
        var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        var uri = $"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&units=metric&lang={language}&appid={apiKey}";

        Forecast forecast = await ReadWebApiAsync(uri);

        //part of event and cache code here
        //generate an event with different message if cached data
        //Your code

        // Cache the new data with the current timestamp
        _cachedGeoForecasts[(latitude, longitude, language)] = (forecast, DateTime.Now);

        // Message that the forecast is not from cache (from the OpenWeatherAPI server).
        OnWeatherForecastAvailable($"Weather forecast for coordinates ({latitude}, {longitude}) is available from server.");

        return forecast;
    }
    private async Task<Forecast> ReadWebApiAsync(string uri)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(uri);
        response.EnsureSuccessStatusCode();

        //Convert Json to NewsResponse
        string content = await response.Content.ReadAsStringAsync();
        WeatherApiData wd = JsonConvert.DeserializeObject<WeatherApiData>(content);

        //Convert WeatherApiData to Forecast using Linq.
        //Your code

        // Format the data from the API to the Forecast model
        var forecast = new Forecast
        {
            City = wd.city.name,
            Items = wd.list.Select(item => new ForecastItem
            {
                DateTime = UnixTimeStampToDateTime(item.dt),
                Temperature = item.main.temp,
                WindSpeed = item.wind.speed,
                Description = item.weather.FirstOrDefault().description,
                Icon = $"http://openweathermap.org/img/w/{item.weather.First().icon}.png"
            }).ToList()
        };
        return forecast;
    }

    private DateTime UnixTimeStampToDateTime(double unixTimeStamp) => DateTime.UnixEpoch.AddSeconds(unixTimeStamp).ToLocalTime();

    // Clear Cache (used for testing purposes)
    public void ClearCache()
    {
        _cachedGeoForecasts.Clear();
    _cachedCityForecasts.Clear();
    Console.WriteLine("Cache cleared.");
    }



}

