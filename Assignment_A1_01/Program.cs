using Assignment_A1_01.Models;
using Assignment_A1_01.Services;

namespace Assignment_A1_01;

class Program
{
    static async Task Main(string[] args)
    {
        //double latitude = 59.5086798659495;
        double latitude = 60.674722;

        //double longitude = 18.2654625932976;
        double longitude = 17.144444;


        Forecast forecast = await new OpenWeatherService().GetForecastAsync(latitude, longitude);

        //Your Code to present each forecast item in a grouped list
        Console.WriteLine($"Weather forecast for {forecast.City}");
        //Console.WriteLine($""




        // Date from dt in UnixTimeStamp format converted to DateTime
        var groupByDate = forecast.Items.GroupBy(x => x.DateTime.Date);

        foreach (var date in groupByDate)
        {
            Console.WriteLine($"Date: {date.Key.ToShortDateString()}");
            foreach (var hour in date)
            {
                Console.WriteLine($"Time: {hour.DateTime.ToLocalTime().ToShortTimeString()}\n" +
                    $"  Temp: {hour.Temperature}\n" +
                    $"  Wind speed: {hour.WindSpeed}\n" +
                    $"  Condition: {hour.Description}\n" +
                    $"  Icon: {hour.Icon}");

            }
            Console.WriteLine();
        }
    }
}

