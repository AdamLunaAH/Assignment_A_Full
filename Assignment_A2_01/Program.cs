using Assignment_A2_01.Models;
using Assignment_A2_01.Services;

namespace Assignment_A2_01;

class Program
{
    static async Task Main(string[] args)
    {
        NewsResponse news = await new NewsService().GetNewsAsync(NewsCategory.Sports);






        Console.WriteLine($"{news.Category} Headlines");

        foreach (var item in news.Articles) 
        {
            Console.WriteLine($"***********************\n" +
                $"Title: {item.Title}\n" +
                $"  Story: {item.Description}\n" +
                $"  Publication: {item.Providers}\n" +
                $"  Date: {item.DatePublished}\n" +
                $"  URL: {item.Url}\n" +
                $"  Image: {item.Image}");

            Console.WriteLine("***********************\n\n");

        }


        //foreach (var date in groupByDate)
        //{
        //    Console.WriteLine($"Date: {date.Key.ToShortDateString()}");
        //    foreach (var hour in date)
        //    {
        //        Console.WriteLine($"Time: {hour.DateTime.ToLocalTime().ToShortTimeString()}\n" +
        //            $"  Temp: {hour.Temperature}\n" +
        //            $"  Wind speed: {hour.WindSpeed}\n" +
        //            $"  Condition: {hour.Description}\n" +
        //            $"  Icon: {hour.Icon}");

        //    }
        //    Console.WriteLine($"Date: {date.Key.ToShortDateString}");
        //    Console.WriteLine($"Title: {date.}");

        //    Console.WriteLine();
        //}


    }
}

