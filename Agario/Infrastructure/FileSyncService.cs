using System.Security.Cryptography;
using Agario.Infrastructure.Utilities;

namespace Agario.Infrastructure;

public static class FileSyncService
{
    public static void SyncFolders(string sourceRelativePath, string destinationFolderName, string filePattern = "*.*")
    {
        string sourceFolderPath = Path.Combine(SolutionPathUtility.GetSolutionPath(), sourceRelativePath);
        string destinationFolderPath = Path.Combine(AppContext.BaseDirectory, destinationFolderName);

        if (!Directory.Exists(sourceFolderPath))
        {
            Console.WriteLine($"Warning: Source folder '{sourceFolderPath}' does not exist.");
            return;
        }

        if (!Directory.Exists(destinationFolderPath))
        {
            Directory.CreateDirectory(destinationFolderPath);
        }

        foreach (var sourceFile in Directory.GetFiles(sourceFolderPath, filePattern, SearchOption.AllDirectories))
        {
            string relativePath = Path.GetRelativePath(sourceFolderPath, sourceFile);
            string destinationFile = Path.Combine(destinationFolderPath, relativePath);

            EnsureFileExists(sourceFile, destinationFile);
        }
    }

    private static void EnsureFileExists(string sourceFile, string destinationFile)
    {
        string? destDir = Path.GetDirectoryName(destinationFile);
        if (destDir != null && !Directory.Exists(destDir))
        {
            Directory.CreateDirectory(destDir);
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
    }
}
