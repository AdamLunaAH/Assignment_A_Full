namespace A1_Weather_Assignment.Models;

public class ForecastItem
{
    public DateTime DateTime { get; set; }
    public double Temperature { get; set; }
    public double WindSpeed { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }
}

