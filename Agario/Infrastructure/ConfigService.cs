using System.Globalization;
using System.Reflection;
using Agario.Game.Configs;
using Agario.Game.Interfaces;

namespace Agario.Infrastructure;

public static class ConfigService
{
    private static readonly string ExecutableConfigDirectory = AppContext.BaseDirectory;
    
    public static void LoadStaticConfig(Type configType, string fileName)
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
                if (field.FieldType == typeof(Dictionary<string, AnimationConfig>))
                    continue;
                if (field.FieldType == typeof(List<string>))
                    SetStaticListValue(field, value);
                else
                    SetStaticValue(field, value);
            }
        }
    }

    public static IConfig LoadInstanceConfig(IConfig config, string fullPath)
    {
        Dictionary<string, List<string>> configData = ReadFile(fullPath);
        
        foreach (var field in config.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!configData.TryGetValue(field.Name, out List<string> values) || values == null)
                continue;

            foreach (var value in values)
            {
                if (field.FieldType == typeof(List<string>))
                    SetInstanceListValue(config, field, value);
                else
                    SetInstanceValue(config, field, value);
            }
        }

        return config;
    }

    private static void SetStaticValue(FieldInfo field, string value)
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
    
    private static void SetInstanceValue(IConfig config, FieldInfo field, string value)
    {
        if (field.FieldType == typeof(uint) && uint.TryParse(value, out uint uintValue))
            field.SetValue(config, uintValue);
        else if (field.FieldType == typeof(int) && int.TryParse(value, out int intValue))
            field.SetValue(config, intValue);
        else if (field.FieldType == typeof(bool) && bool.TryParse(value, out bool boolValue))
            field.SetValue(config, boolValue);
        else if (field.FieldType == typeof(float) && float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatValue))
            field.SetValue(config, floatValue);
        else if (field.FieldType == typeof(string))
            field.SetValue(config, value);
    }

    private static void SetStaticListValue(FieldInfo field, string value)
    {
        if (field.GetValue(null) == null)
            field.SetValue(null,new List<string>());

        if (field.GetValue(null) is List<string> list)
        {
            list.Add(value);
        }
    }
    
    private static void SetInstanceListValue(IConfig config, FieldInfo field, string value)
    {
        if (field.GetValue(config) == null)
            field.SetValue(config,new List<string>());

        if (field.GetValue(config) is List<string> list)
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