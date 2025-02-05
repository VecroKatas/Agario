using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using Agario.Game.Configs;
using Agario.Infrastructure.Utilities;

namespace Agario.Infrastructure;

public static class ConfigService
{
    private static readonly string ExecutableConfigDirectory = AppContext.BaseDirectory;
    private static readonly string SolutionConfigDirectory = SolutionPathUtility.GetSolutionPath();
    
    /*private static void EnsureConfigExists(string sourceFile, string destinationFile)
    {
        if (!Directory.Exists(ExecutableConfigDirectory))
        {
            Directory.CreateDirectory(ExecutableConfigDirectory);
        }
        
        if (!File.Exists(sourceFile))
        {
            return;
        }

        if (!File.Exists(destinationFile) || !FilesAreEqual(sourceFile, destinationFile))
        {
            File.Copy(sourceFile, destinationFile, true);
        }
    }

    private static bool FilesAreEqual(string file1, string file2)
    {
        using var md5 = MD5.Create();
        using var stream1 = File.OpenRead(file1);
        using var stream2 = File.OpenRead(file2);
        return BitConverter.ToString(md5.ComputeHash(stream1)) == BitConverter.ToString(md5.ComputeHash(stream2));
    }*/
    
    public static void LoadConfig(Type configType, string fileName)
    {
        /*string solutionConfigIniPath = Path.Combine(SolutionConfigDirectory, fileName);
        string executableConfigIniPath = Path.Combine(ExecutableConfigDirectory, fileName);

        EnsureConfigExists(solutionConfigIniPath, executableConfigIniPath);*/

        string path;
        if (EntryPointConfig.ConfigsFolder == null)
            path = Path.Combine(ExecutableConfigDirectory, fileName);
        else
            path = Path.Combine(ExecutableConfigDirectory, EntryPointConfig.ConfigsFolder, fileName);
        
        Dictionary<string, string> configData = ReadFile(path);
        
        foreach (var field in configType.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            if (!configData.TryGetValue(field.Name, out string value) || string.IsNullOrEmpty(value))
                continue;

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
    }

    private static Dictionary<string, string> ReadFile(string filePath)
    {
        Dictionary<string, string> configData = new();

        foreach (var line in File.ReadAllLines(filePath))
        {
            string trimmedLine = line.Trim();
            if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith(";") || trimmedLine.StartsWith("#"))
                continue;

            var keyValue = trimmedLine.Split('=', 2);
            if (keyValue.Length == 2)
            {
                configData[keyValue[0].Trim()] = keyValue[1].Trim();
            }
        }

        return configData;
    }
}