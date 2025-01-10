using Assignment_A1_03.Models;
using Assignment_A1_03.Services;

namespace Assignment_A1_03;

class Program
{
    static async Task Main(string[] args)
    {
        // Create an instance of the OpenWeatherService
        OpenWeatherService service = new OpenWeatherService();

        //Register the event
        //Your Code

        // Register the WeatherForecastAvailable event handler
        service.WeatherForecastAvailable += WeatherForecastHandler;

        // Clear any existing cache (for testing)
        //service.ClearCache();

        // Array to hold the tasks
        Task<Forecast>[] tasks = { null, null, null, null };
        Exception exception = null;
        try
        {
            // Location in coordinates
            double latitude = 60.67452;
            double longitude = 17.14174;

            // Location in name
            string location = "Borlänge";

            // Create and start the two tasks 
            tasks[0] = service.GetForecastAsync(latitude, longitude);
            tasks[1] = service.GetForecastAsync(location);

            // Wait for the tasks to complete
            await Task.WhenAll(tasks[0], tasks[1]);

            // Clear cache between tasks (for testing)
            //service.ClearCache();
            // Pause used for testing the cache time check (the changed DateTime in OpenWeatherService is better to use for testing old data)  
            //await Task.Delay(100000);

            // Create and start the two tasks
            tasks[2] = service.GetForecastAsync(latitude, longitude);
            tasks[3] = service.GetForecastAsync(location);

            // Wait for the tasks to complete, and use the cache for fetching the data
            await Task.WhenAll(tasks[2], tasks[3]);

            Console.WriteLine("\nPress a button to show the weather data.\n");
            Console.ReadKey();

        }
        catch (Exception ex)
        {
            exception = ex;
            //How to handle an exception
            //Your Code

            // Log error messages to the console
            Console.WriteLine($"An error occured: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
        }

        // Loop through the tasks to check their status and display the forecast
        foreach (var task in tasks)
        {
            //How to deal with successful and fault tasks
            //Your Code

            if (task == null)
            {
                Console.WriteLine("Task is null");
                continue;
            }

            // Check the status of the task and display the forecast or error message
            switch (task.Status)
            {
                // Successful task
                case TaskStatus.RanToCompletion:
                    //Console.WriteLine("Task completed successfully");
                    // Runs the DisplayForecast method which displays the forecast
                    DisplayForecast(task.Result);
                    break;
                // Faulted task
                case TaskStatus.Faulted:
                    Console.WriteLine($"Task faulted with exception:");
                    foreach (var innerEx in task.Exception?.InnerExceptions ?? Enumerable.Empty<Exception>())
                    {
                        Console.WriteLine($" - {innerEx.Message}");
                    }
                    break;
                // Canceled task
                case TaskStatus.Canceled:
                    Console.WriteLine("Task was canceled.");
                    break;
                // Other/unexpected task faults
                default:
                    Console.WriteLine($"Task is in an unexpected state: {task.Status}");
                    break;
            }

            

        }

    }


    //Event handler declaration
    //Your Code

    // Event handler for the WeatherForecastAvailable event and present a message if the forecast is available
    static void WeatherForecastHandler(object sender, string message)
    {
        // Message for if the forecast is available
        Console.WriteLine($"Weather forecast available: {message}");
    }

    // Method to display the forecast
    static void DisplayForecast(Forecast forecast)
    {
        Console.WriteLine($"Weather forecast for {forecast.City}");
        foreach (var item in forecast.Items.GroupBy(x => x.DateTime.Date))
        {
            Console.WriteLine($"***********************\n" 
                + $"{forecast.City}\n" +
                $"Date: {item.Key.ToShortDateString()}");

            foreach (var hour in item)
            {
                Console.WriteLine($"    Time: {hour.DateTime.ToLocalTime().ToShortTimeString()}\n" +
                    $"      Temp: {hour.Temperature}\n" +
                    $"      Wind speed: {hour.WindSpeed}\n" +
                    $"      Condition: {hour.Description}\n" +
                    $"      Icon: {hour.Icon}");
            }
            Console.WriteLine("***********************\n");
        }
    }


}

