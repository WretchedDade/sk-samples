namespace SemanticKernelSamples.Helpers;
public static class FileHelper
{
    public static string BuildPathFromBaseDirectory(params string[] parts)
    {
        string directoryPath = Path.Combine([AppContext.BaseDirectory, ..parts]);

        if (!Directory.Exists(directoryPath))
            throw new DirectoryNotFoundException($"Directory '{directoryPath}' not found in the base directory.");

        return directoryPath;
    }

    public static string LoadPrompt(string fileName)
    {
        if (!fileName.EndsWith(".yml"))
            fileName = $"{fileName}.yml";

        string filePath = Path.Combine(AppContext.BaseDirectory, "Prompts", fileName);

        return !File.Exists(filePath)
            ? throw new FileNotFoundException($"Prompt file '{fileName}' not found in the 'Prompts' directory.")
            : File.ReadAllText(filePath);
    }
}
