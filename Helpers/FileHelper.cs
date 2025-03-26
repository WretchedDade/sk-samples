namespace SemanticKernelSamples.Helpers;
public static class FileHelper
{
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
