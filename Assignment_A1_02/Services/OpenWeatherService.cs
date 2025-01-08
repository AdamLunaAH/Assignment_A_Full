using Assignment_A1_02.Models;
using Newtonsoft.Json;

namespace Assignment_A1_02.Services;
public class OpenWeatherService
{
    readonly HttpClient _httpClient = new HttpClient();
    readonly string _apiKey = "d11de2c96e160e2d3350ad3db04c75bc";

    //Event declaration
    public event EventHandler<string> WeatherForecastAvailable;
    protected virtual void OnWeatherForecastAvailable (string message)
    {
        WeatherForecastAvailable?.Invoke(this, message);
    }
    public async Task<Forecast> GetForecastAsync(string City)
    {
        //https://openweathermap.org/current
        var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        var uri = $"https://api.openweathermap.org/data/2.5/forecast?q={City}&units=metric&lang={language}&appid={_apiKey}";


        Forecast forecast = await ReadWebApiAsync(uri);


        //Event code here to fire the event
        //Your code
        OnWeatherForecastAvailable($"Weather forecast for {City} is available.");

        return forecast;
    }
    public async Task<Forecast> GetForecastAsync(double latitude, double longitude)
    {
        //https://openweathermap.org/current
        var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        var uri = $"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&units=metric&lang={language}&appid={_apiKey}";

        Forecast forecast = await ReadWebApiAsync(uri);

        //Event code here to fire the event
        //Your code
        OnWeatherForecastAvailable($"Weather forecast for coordinates ({latitude}, {longitude}) is available.");

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
}

