namespace Agario.Infrastructure.Utilities;

public static class SolutionPathUtility
{
    public static string GetSolutionPath()
    {
        string currentDirectory = Directory.GetCurrentDirectory();

        while (!string.IsNullOrEmpty(currentDirectory))
        {
            if (Directory.GetFiles(currentDirectory, "*.sln").Length > 0)
            {
                return currentDirectory;
            }

            currentDirectory = Directory.GetParent(currentDirectory).FullName;
        }

        return "";
    }
}