namespace Assignment_A2_04.Models;

public class NewsCacheKey
{
    readonly NewsCategory _category;
    readonly string _timewindow;

    public string FileName => fname("Cache-" + Key + ".xml");
    // Changed the file name to a "-" between the category and the time window to make the "delete old cache"-function in LoadCacheFromXML-funcion to work.
    public string Key => _category.ToString() + "-" + _timewindow;
    public bool CacheExist => File.Exists(FileName);

    public NewsCacheKey(NewsCategory category, DateTime dt)
    {
        _category = category;
        _timewindow = DateTime.Now.ToString("yyyy-MM-dd-HH-mm");
    }
    private static string fname(string name)
    {
        var documentPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        documentPath = Path.Combine(documentPath, "ADOP", "ProjectB");
        if (!Directory.Exists(documentPath)) Directory.CreateDirectory(documentPath);
        return Path.Combine(documentPath, name);
    }

    // Get the cache directory instead of the file name for use in LoadCacheFromXML
    public static string GetCacheDirectory()
    {
        var documentPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(documentPath, "ADOP", "ProjectB");
    }

}