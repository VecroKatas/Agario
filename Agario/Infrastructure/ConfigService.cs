using System.Globalization;
using System.Reflection;
using Agario.Game.Configs;

namespace Agario.Infrastructure;

public static class ConfigService
{
    private static readonly string ExecutableConfigDirectory = AppContext.BaseDirectory;
    
    public static void LoadConfig(Type configType, string fileName)
    {
        string path;
        if (EntryPointConfig.ConfigsFolder == null)
            path = Path.Combine(ExecutableConfigDirectory, fileName);
        else
            path = Path.Combine(ExecutableConfigDirectory, EntryPointConfig.ConfigsFolder, fileName);
        
        Dictionary<string, List<string>> configData = ReadFile(path);
        
        foreach (var field in configType.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            if (!configData.TryGetValue(field.Name, out List<string> values) || values == null)
                continue;

            foreach (var value in values)
            {
                if (field.FieldType == typeof(List<string>))
                    SetListValue(field, value);
                else
                    SetValue(field, value);
            }
        }
    }

    private static void SetValue(FieldInfo field, string value)
    {
        if (field.FieldType == typeof(uint) && uint.TryParse(value, out uint uintValue))
            field.SetValue(null, uintValue);
        else if (field.FieldType == typeof(int) && int.TryParse(value, out int intValue))
            field.SetValue(null, intValue);
        else if (field.FieldType == typeof(bool) && bool.TryParse(value, out bool boolValue))
            field.SetValue(null, boolValue);
        else if (field.FieldType == typeof(float) && float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatValue))
            field.SetValue(null, floatValue);
        else if (field.FieldType == typeof(string))
            field.SetValue(null, value);
    }

    private static void SetListValue(FieldInfo field, string value)
    {
        if (field.GetValue(null) == null)
            field.SetValue(null,new List<string>());

        if (field.GetValue(null) is List<string> list)
        {
            list.Add(value);
        }
    }

    private static Dictionary<string, List<string>> ReadFile(string filePath)
    {
        Dictionary<string, List<string>> configData = new();

        foreach (var line in File.ReadAllLines(filePath))
        {
            string trimmedLine = line.Trim();
            if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith(";") || trimmedLine.StartsWith("#"))
                continue;

            var keyValue = trimmedLine.Split('=', 2);
            if (keyValue.Length == 2)
            {
                if (!configData.ContainsKey(keyValue[0].Trim()))
                    configData[keyValue[0].Trim()] = new List<string>();
                configData[keyValue[0].Trim()].Add(keyValue[1].Trim());
            }
        }

        return configData;
    }
}