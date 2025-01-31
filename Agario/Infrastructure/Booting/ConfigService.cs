namespace Agario.Infrastructure;

public static class ConfigService
{
    private static readonly Dictionary<string, string> _configData = new();

    public static void LoadConfig(string filePath)
    {
        _configData.Clear();

        foreach (var line in File.ReadAllLines(filePath))
        {
            string trimmedLine = line.Trim();
            if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith(";") || trimmedLine.StartsWith("#"))
                continue;

            var keyValue = trimmedLine.Split('=', 2);
            if (keyValue.Length == 2)
            {
                _configData[keyValue[0].Trim()] = keyValue[1].Trim();
            }
        }
    }

    public static string GetString(string key, string defaultValue = "")
    {
        return _configData.TryGetValue(key, out var value) ? value : defaultValue;
    }
}