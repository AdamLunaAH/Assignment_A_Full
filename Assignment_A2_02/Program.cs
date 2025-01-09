using Assignment_A2_02.Models;
using Assignment_A2_02.Services;

namespace Assignment_A2_02;

class Program
{
    static async Task Main(string[] args)
    {

        //Register the event

        NewsService service = new NewsService();


        service.NewsAvailable += NewsHandler;

        var categories = Enum.GetValues(typeof(NewsCategory)).Cast<NewsCategory>();

        //var tasks = categories.Select(category => service.GetNewsAsync(category)).ToArray();
        //Task<NewsResponse>[] tasks = { null, null, null, null, null };

        Exception exception = null;

        var tasks = new List<Task<NewsResponse>>();

        foreach (var category in categories)
        {
            try
            {
                tasks.Add(service.GetNewsAsync(category));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create task for category {category}: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }
        }



        try
        {
            //tasks[0] = service.GetNewsAsync(NewsCategory.Sports);
            //tasks[1] = service.GetNewsAsync(NewsCategory.Technology);
            //tasks[2] = service.GetNewsAsync(NewsCategory.Business);
            //tasks[3] = service.GetNewsAsync(NewsCategory.Entertainment);
            //tasks[4] = service.GetNewsAsync(NewsCategory.World);


            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            exception = ex;
            //How to handle an exception
            Console.WriteLine($"An error occured: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
        }

        foreach (var task in tasks)
        {
            //How to deal with successful and fault tasks

            if (task == null)
            {
                Console.WriteLine("Task is null");
                continue;
            }

            switch (task.Status)
            {
                // Successful task
                case TaskStatus.RanToCompletion:
                    Console.WriteLine("Task completed successfully");
                    DisplayNews(task.Result);
                    break;

                case TaskStatus.Faulted:
                    Console.WriteLine($"Task faulted with exception:");
                    foreach (var innerEx in task.Exception?.InnerExceptions ?? Enumerable.Empty<Exception>())
                    {
                        Console.WriteLine($" - {innerEx.Message}");
                    }
                    break;
                case TaskStatus.Canceled:
                    Console.WriteLine("Task was canceled.");
                    break;
                default:
                    Console.WriteLine($"Task is in an unexpected state: {task.Status}");
                    break;
            }

        }
    }


    static void NewsHandler(object sender, string message)
    {
        Console.WriteLine($"News is available: {message}");
    }


    static void DisplayNews(NewsResponse news)
    {
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
        Console.WriteLine($"***********************");

        Console.WriteLine($"{news.Category} Headlines");
        foreach (var item in news.Articles)
        {  
            Console.WriteLine($"    - {item.DatePublished}: {item.Title}"); 
        }

        Console.WriteLine($"***********************\n");

    }
}

