using A2_News_Assignment.Models;
using A2_News_Assignment.Services;

namespace A2_News_Assignment;

class Program
{
    static async Task Main(string[] args)
    {
        // Create an instance of the NewsService
        NewsService service = new NewsService();

        // Register the NewsAvailable event handler
        service.NewsAvailable += NewsHandler;

        // Retrieve news for each category
        var categories = Enum.GetValues(typeof(NewsCategory)).Cast<NewsCategory>();

        // Array to hold the tasks (used when the CreateTaskLoop is not used)
        //Task<NewsResponse>[] tasks = { null, null, null, null, null, null, null, null, null, null };


        // Create a list for tasks (used when the CreateTaskLoop is used)
        var tasks = new List<Task<NewsResponse>>();

        Exception exception = null;

        // CreateTaskLoop - Create tasks for fetching news from each category
        // Loop through each category and create a task for fetching news
        foreach (var category in categories)
        {
            try
            {
                // Create a task for fetching news from a category
                tasks.Add(service.GetNewsAsync(category));
            }
            catch (Exception ex)
            {
                // Handle exceptions when creating tasks
                Console.WriteLine($"Failed to create task for category {category}: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }
        }



        try
        {
            // Create and start the tasks manually (if tasks are created with this, the CreateTaskLoop is not needed)
            //tasks[0] = service.GetNewsAsync(NewsCategory.Sports);
            //tasks[1] = service.GetNewsAsync(NewsCategory.Technology);
            //tasks[2] = service.GetNewsAsync(NewsCategory.Business);
            //tasks[3] = service.GetNewsAsync(NewsCategory.Entertainment);
            //tasks[4] = service.GetNewsAsync(NewsCategory.World);
            // Wait for the tasks to complete (used when manually creating the tasks)
            //await Task.WhenAll(tasks[0], tasks[1], tasks[2], tasks[3], tasks[4]);

            // Wait for the tasks to complete (used when CreateTaskLoop is used)
            await Task.WhenAll(tasks);

            // Clears the cache (used for testing)
            //service.ClearCache();

        }
        catch (Exception ex)
        {
            exception = ex;
            // Handle exceptions when waiting for tasks to complete
            // Log error messages to the console
            Console.WriteLine($"An error occured: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
        }

        // Loop through the tasks to check their status and display the news
        foreach (var task in tasks)
        {
            if (task == null)
            {
                Console.WriteLine("Task is null");
                continue;
            }

            // Check the status of the task and display the news or error message
            switch (task.Status)
            {
                // Successful task
                case TaskStatus.RanToCompletion:
                    //Console.WriteLine("Task completed successfully");
                    // Runs the DisplayForecast method which displays the news
                    DisplayNews(task.Result);
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

    // Event handler for the NewsAvailable event and present a message if the news is available
    static void NewsHandler(object sender, string message)
    {
        // Message for if the news is available
        Console.WriteLine($"News is available: {message}");
    }

    // Method to display the news
    static void DisplayNews(NewsResponse news)
    {
        // Display the full news data
        //Console.WriteLine($"{news.Category} Headlines");
        //foreach (var item in news.Articles)
        //{
        //    Console.WriteLine($"***********************\n" +
        //        $"Title: {item.Title}\n" +
        //        $"  Story: {item.Description}\n" +
        //        $"  Publication: {item.Providers}\n" +
        //        $"  Date: {item.DatePublished}\n" +
        //        $"  URL: {item.Url}\n" +
        //        $"  Image: {item.Image}");
        //    Console.WriteLine("***********************\n\n");
        //}

        // Display selected news data
        Console.WriteLine($"***********************");
        Console.WriteLine($"{news.Category} Headlines");
        foreach (var item in news.Articles)
        {
            Console.WriteLine($"    - {item.DatePublished}: {item.Title}");
        }
        Console.WriteLine($"***********************\n");

    }
}

